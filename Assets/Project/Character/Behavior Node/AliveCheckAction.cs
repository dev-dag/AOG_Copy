using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Dead Check", story: "Success When [Archer] Dead", category: "Archer", id: "48df0f352afe790be38cac5ea5f32620")]
public partial class AliveCheckAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;

    protected override Status OnStart()
    {
        if (Archer.Value.State == global::Archer.AnimationEnum.Die)
        {
            return Status.Success;
        }
        else if (Archer.Value.CurrentHP <= 0f)
        {
            Archer.Value.DoDie();

            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }
}

