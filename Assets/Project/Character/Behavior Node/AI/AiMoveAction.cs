using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AI_Move", story: "AI [Archer] Move With [Min] [Max]", category: "Archer/AI", id: "37ee0adec0d9fa3209103757a247ffde")]
public partial class AiMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;
    [SerializeReference] public BlackboardVariable<float> Min;
    [SerializeReference] public BlackboardVariable<float> Max;

    private Vector2 point;

    protected override Status OnStart()
    {
        point = new Vector2(GetPointX(), Archer.Value.transform.position.y);
        Archer.Value.DoWalk();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Mathf.Abs(Archer.Value.transform.position.x - point.x) < 0.1f)
        {
            return Status.Success;
        }
        else
        {
            int direction = point.x - Archer.Value.transform.position.x < 0 ? -1 : 1;
            Archer.Value.Look(direction);

            Archer.Value.RigidBody.MovePosition(Archer.Value.transform.position + Vector3.right * direction * Archer.Value.Speed * Time.deltaTime);

            return Status.Running;
        }
    }

    protected override void OnEnd()
    {
        Archer.Value.DoIdle();
    }

    private float GetPointX()
    {
        float dist = Max.Value - Min.Value;
        float randomValue = UnityEngine.Random.Range(0f, dist);
        return Min.Value + randomValue;
    }
}

