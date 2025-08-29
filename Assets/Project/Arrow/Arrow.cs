using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class Arrow : MonoBehaviour
{
    public bool IsHit { get => isHit; }

    [SerializeField] protected Transform renderTransform;
    [SerializeField] protected Rigidbody2D rigidBody;
    [SerializeField] protected SpriteRenderer render;

    [Space(15f)]
    [SerializeField] protected Vector2 startPos;
    [SerializeField] protected Vector2 endPos;
    [SerializeField] protected float maxY;
    [SerializeField] protected float speed;
    [SerializeField] protected Archer shooter;
    [SerializeField] protected bool isHit;
    [SerializeField] protected int damage;

    protected CancellationTokenSource cancelToken;
    protected ObjectPool<object> pool;
    protected ObjectPool<object> fxPool;

    private void Awake()
    {
        
    }

    public void Initialize(ObjectPool<object> newPool)
    {
        pool = newPool;
    }

    public virtual void Shoot(Archer newShooter, int newDamage, Transform startTransform, Transform endTransform, ObjectPool<object> newFX_Pool = null, float newMaxY = 5f, float newSpeed = 1f, float arrowRotOffsetZ = 0f)
    {
        startPos = startTransform.position;
        endPos = endTransform.position;
        maxY = newMaxY;
        speed = newSpeed;
        shooter = newShooter;
        renderTransform.localRotation = Quaternion.Euler(0f, 0f, arrowRotOffsetZ);
        damage = newDamage;
        render.color = Color.white;
        isHit = false;
        fxPool = newFX_Pool;
        this.transform.rotation = Quaternion.identity;
        this.transform.position = startTransform.position;

        if (cancelToken != null)
        {
            cancelToken.Cancel();
        }

        cancelToken = new CancellationTokenSource();

        MoveArrow(cancelToken.Token);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == shooter.Collider.gameObject)
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

    protected virtual void OnHit(Collider2D collider)
    {
        isHit = true;

        DeleteProc();

        var archer = collider.gameObject.GetComponentInParent<Archer>();
        if (archer != null)
        {
            archer.TakeHit(damage);
        }

        if (fxPool != null)
        {
            PoolingEffect effect = (PoolingEffect)(fxPool.Get());
            effect.transform.position = archer.transform.position;
            effect.Initialize(pool);
        }
    }

    protected virtual async Awaitable MoveArrow(CancellationToken ct)
    {
        float r1 = startPos.x;
        float r2 = endPos.x;
        float m = (r1 + r2) / 2f; // 두 근의 중간 값

        float a = maxY / ((m - r1) * (m - r2));

        int direction = endPos.x - startPos.x < 0f ? -1 : 1;

        Func<float, float, float, float, float> yFunc = (a, x, m, y) =>
        {
            var result = a * MathF.Pow(x - m, 2) + y;

            return result;
        };

        while (this.transform.position.y > -20f)
        {
            ct.ThrowIfCancellationRequested();

            float newPosX = this.transform.position.x + direction * speed * Time.deltaTime;
            float newDeltaY = yFunc(a, newPosX, m, maxY);

            Vector3 newPos = new Vector2(newPosX, startPos.y + newDeltaY);

            this.transform.rotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.down, transform.position - newPos));

            rigidBody.MovePosition(newPos);

            await Awaitable.NextFrameAsync();
        }
    }

    protected virtual async Awaitable DeleteProc()
    {
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
