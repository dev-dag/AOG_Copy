using UnityEngine;

public class IceArrow : Arrow
{
    [SerializeField] private float frozenTime = 3f;

    protected override void OnHit(Collider2D collider)
    {
        base.OnHit(collider);

        var archer = collider.gameObject.GetComponentInParent<Archer>();
        if (archer != null)
        {
            archer.ConditionControl.SetCondition(ConditionControl.ConditionEnum.Frozen, Time.time + frozenTime);
        }
    }
}