using System;
using Unity.Behavior;
using UnityEngine;
using Modifier = Unity.Behavior.Modifier;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DeadCheck", story: "Check [Archer] Dead", category: "Archer", id: "af5b4e0a7f4461cfdae4f6ea472c3dbf")]
public partial class DeadCheckModifier : Modifier
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
            return StartNode(Child);
        }
    }

    protected override Status OnUpdate()
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

