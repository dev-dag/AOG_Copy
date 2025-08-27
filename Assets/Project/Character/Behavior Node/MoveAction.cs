using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move", story: "[Archer] Move By [GameSceneControl] 's InputData", category: "Action", id: "60dc18136676f2bd0a381fb653a0d791")]
public partial class MoveAction : Action
{
    [SerializeReference] public BlackboardVariable<Archer> Archer;
    [SerializeReference] public BlackboardVariable<GameSceneControl> GameSceneControl;

    protected override Status OnStart()
    {
        if (Archer.Value.State is global::Archer.AnimationEnum.Attack or global::Archer.AnimationEnum.Hurt or global::Archer.AnimationEnum.Die)
        {
            return Status.Failure;
        }
        else
        {
            int xDirection = GameSceneControl.Value.InputData.xDirection;

            if (xDirection == 0)
            {
                Archer.Value.DoIdle();
                return Status.Failure;
            }
            else
            {
                Archer.Value.Look(xDirection);
                Archer.Value.RigidBody.MovePosition(Archer.Value.transform.position + Vector3.right * xDirection * Archer.Value.Speed * Time.deltaTime);

                if (Archer.Value.State != global::Archer.AnimationEnum.Walk)
                {
                    Archer.Value.DoWalk();
                }

                return Status.Success;
            }
        }
    }
}

