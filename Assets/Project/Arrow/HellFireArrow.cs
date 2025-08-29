using UnityEngine;

public class HellFireArrow : Arrow
{
    [Space(15f)]
    [SerializeField] private HellFire hellFireInstance;

    private void Awake()
    {
        hellFireInstance.gameObject.SetActive(false);
    }

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

        hellFireInstance.gameObject.SetActive(true);
        hellFireInstance.Initialize(this.transform.position);
        hellFireInstance.onExitEvent += () => hellFireInstance.gameObject.SetActive(false);
        hellFireInstance.onHitEvent += (hitArcher) => hitArcher.TakeHit(damage);
    }

    public override void Shoot(Archer newShooter, int newDamage, Transform startTransform, Transform endTransform, float newMaxY = 5, float newSpeed = 1, float arrowRotOffsetZ = 0)
    {
        base.Shoot(newShooter, newDamage, startTransform, endTransform, newMaxY, newSpeed, arrowRotOffsetZ);

        hellFireInstance.gameObject.SetActive(false);
    }

    protected override async Awaitable DeleteProc()
    {
        await Awaitable.WaitForSecondsAsync(3f);

        float alpha = 1f;
        float fadeSpeed = 5f;

        while (alpha > 0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;

            render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);

            await Awaitable.NextFrameAsync();
        }

        alpha = 0f;

        pool.Release(this);
    }
}