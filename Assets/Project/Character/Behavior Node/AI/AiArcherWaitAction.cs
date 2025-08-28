using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI_ArcherWait", story: "AI [Archer] Wait Maximum [Sec]", category: "Archer/AI", id: "8304351539e8f974b442fbf7084afeea")]
public partial class AiArcherWaitAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;
    [SerializeReference] public BlackboardVariable<float> Sec;

    private float waitFor;

    protected override Status OnStart()
    {
        waitFor = Time.time + GetWaitTime();
        Archer.Value.DoIdle();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Time.time <  waitFor)
        {
            return Status.Running;
        }
        else
        {
            return Status.Success;
        }
    }

    private float GetWaitTime()
    {
        return UnityEngine.Random.Range(0, Sec.Value);
    }
}

