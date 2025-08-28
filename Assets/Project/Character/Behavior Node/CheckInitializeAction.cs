using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Initialize", story: "Success When [Archer] Initialized", category: "Archer", id: "0e5f6ba2ebb2191f0a0c11b650e3085d")]
public partial class CheckInitializeAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;
    protected override Status OnStart()
    {
        if (Archer.Value.DoBehavior)
        {
            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }
}

