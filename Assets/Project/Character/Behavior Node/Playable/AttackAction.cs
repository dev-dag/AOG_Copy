using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Archer] Attack By [GameSceneControl] 's InputData", category: "Archer/Playable", id: "19d5cc555f25f7f183a374c8fe63f591")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;
    [SerializeReference] public BlackboardVariable<GameSceneControl> GameSceneControl;
    protected override Status OnStart()
    {
        if (Archer.Value.State == global::Archer.AnimationEnum.Idle)
        {
            Archer.Value.Look(1);
            Archer.Value.DoAttack();

            return Status.Running;
        }
        else
        {
            return Status.Failure;
        }
    }

    protected override Status OnUpdate()
    {
        var animator = Archer.Value.Animator;
        var currentAnimationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (currentAnimationStateInfo.shortNameHash != global::Archer.AnimationHash.ATTACK) // 상태머신이 갱신되지 않은 경우
        {
            if (GameSceneControl.Value.InputData.xDirection != 0)
            {
                return Status.Success;
            }

            return Status.Running;
        }
        else if (currentAnimationStateInfo.normalizedTime < 1f)
        {
            if (currentAnimationStateInfo.normalizedTime < 0.2f && GameSceneControl.Value.InputData.xDirection != 0) // 공격 캔슬
            {
                return Status.Failure;
            }

            return Status.Running;
        }
        else
        {
            return Status.Success;
        }
    }

    protected override void OnEnd()
    {
        Archer.Value.DoIdle();
    }
}

