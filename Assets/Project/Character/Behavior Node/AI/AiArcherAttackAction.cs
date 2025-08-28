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

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Archer.Value.Look(-1);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

