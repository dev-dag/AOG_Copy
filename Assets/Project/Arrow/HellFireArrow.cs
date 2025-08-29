using UnityEngine;

public class HellFireArrow : Arrow
{
    [Space(15f)]
    [SerializeField] private HellFire hellFirePrefab;
    [SerializeField] private HellFire hellFireInstance;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == shooter.Collider.gameObject)
        {
            return;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(LayerDefine.ARCHER)) // 헬파이어 화살은 플랫폼에만 상호작용함.
        {
            return;
        }
        else if (isHit)
        {
            return;
        }

        OnHit(collision);

        cancelToken.Cancel();
    }

    protected override void OnHit(Collider2D collider)
    {
        isHit = true;

        DeleteProc();

        if (hellFireInstance == null)
        {
            hellFireInstance = GameObject.Instantiate<HellFire>(hellFirePrefab);
        }

        hellFireInstance.gameObject.SetActive(true);
        hellFireInstance.Initialize(this.transform.position);
        hellFireInstance.onExitEvent += () => hellFireInstance.gameObject.SetActive(false);
    }
}