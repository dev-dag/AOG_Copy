using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckSKillActive", story: "Check [Archer] 's Skill Activated By [GameSceneControl]", category: "Archer/Playable", id: "f146ffdf08385fbb32372c837c3f4fd9")]
public partial class CheckSKillActiveModifier : Modifier
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;
    [SerializeReference] public BlackboardVariable<GameSceneControl> GameSceneControl;

    private int skillIndexCache;

    protected override Status OnStart()
    {
        if (GameSceneControl.Value.InputData.ActivatedSkillIndex.Value != -1)
        {
            skillIndexCache = GameSceneControl.Value.InputData.ActivatedSkillIndex.Value;

            Archer.Value.DoSkill(skillIndexCache);
            return Status.Running;
        }
        else
        {
            return StartNode(Child);
        }
    }

    protected override Status OnUpdate()
    {
        if (Archer.Value.State == global::Archer.AnimationEnum.Skill)
        {
            var animator = Archer.Value.Animator;
            var currentAnimationStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (currentAnimationStateInfo.tagHash != Animator.StringToHash("Skill")) // 상태머신이 갱신되지 않은 경우
            {
                return Status.Running;
            }
            else if (currentAnimationStateInfo.tagHash == Animator.StringToHash("Skill") && currentAnimationStateInfo.normalizedTime < 1f)
            {
                return Status.Running;
            }
            else if (currentAnimationStateInfo.tagHash == Animator.StringToHash("Skill") && currentAnimationStateInfo.normalizedTime >= 1f)
            {
                if (GameSceneControl.Value.InputData.ActivatedSkillIndex.Value == skillIndexCache) // 예약된 다른 스킬이 없는 경우
                {
                    GameSceneControl.Value.InputData.ActivatedSkillIndex.Value = -1;
                }

                Archer.Value.DoIdle();
                return Status.Success;
            }
            else
            {
                return Status.Running;
            }
        }
        else if (GameSceneControl.Value.InputData.ActivatedSkillIndex.Value != -1)
        {
            skillIndexCache = GameSceneControl.Value.InputData.ActivatedSkillIndex.Value;
            Archer.Value.DoSkill(skillIndexCache);
            return Status.Running;
        }
        else
        {
            if (Child.CurrentStatus is Status.Running or Status.Waiting)
            {
                return Status.Running;
            }
            else
            {
                return Child.CurrentStatus;
            }
        }
    }
}

