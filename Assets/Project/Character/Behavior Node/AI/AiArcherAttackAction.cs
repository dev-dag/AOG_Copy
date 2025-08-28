using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI_ArcherAttack", story: "AI [Archer] Attack", category: "Archer/AI", id: "a3173a3cfce6510ff75d68e638dc18a9")]
public partial class AiArcherAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;

    private int attackCount;
    private int tmpAttackCount;

    protected override Status OnStart()
    {
        attackCount = GetAttackCount();
        tmpAttackCount = 0;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var animator = Archer.Value.Animator;
        var currentAnimationStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (tmpAttackCount >= attackCount)
        {
            if (Archer.Value.State is not global::Archer.AnimationEnum.Attack || currentAnimationStateInfo.normalizedTime < 1f)
            {
                return Status.Running;
            }

            return Status.Success;
        }
        else
        {
            Archer.Value.Look(-1);

            if (Archer.Value.State is not global::Archer.AnimationEnum.Attack)
            {
                Archer.Value.DoAttack();
                tmpAttackCount++;
            }
            else if (currentAnimationStateInfo.shortNameHash == global::Archer.AnimationHash.ATTACK && currentAnimationStateInfo.normalizedTime >= 1f)
            {
                Archer.Value.DoIdle();
            }

            return Status.Running;
        }
    }

    private int GetAttackCount()
    {
        return UnityEngine.Random.Range(1, 5);
    }
}

