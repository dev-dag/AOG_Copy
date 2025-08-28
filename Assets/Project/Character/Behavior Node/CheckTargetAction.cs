using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckTarget", story: "Success When [Archer] 's Target Settes", category: "Archer", id: "044a6b5a86a3494a06319fa8e6300462")]
public partial class CheckTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;

    protected override Status OnStart()
    {
        if (Archer.Value.Target != null)
        {
            if (Archer.Value.Target.State == global::Archer.AnimationEnum.Die)
            {
                if (Archer.Value.State is not global::Archer.AnimationEnum.Victory)
                {
                    Archer.Value.DoVictory();
                }

                return Status.Running;
            }
            else
            {
                return Status.Success;
            }
        }
        else
        {
            return Status.Failure;
        }
    }

    protected override Status OnUpdate()
    {
        if (Archer.Value.Target.State == global::Archer.AnimationEnum.Die)
        {
            return Status.Running;
        }
        else
        {
            return Status.Success;
        }
    }
}

