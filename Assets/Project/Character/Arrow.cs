using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool IsHit { get => isHit; }

    [SerializeField] private float zRotOffset;
    [SerializeField] private Transform renderTransform;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer render;

    [Space(15f)]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float maxY;
    [SerializeField] private float speed;
    [SerializeField] private Archer shooter;
    [SerializeField] private bool isHit;

    protected CancellationTokenSource cancelToken;

    private void Awake()
    {
        
    }

    public virtual void Shoot(Archer newShooter, Transform startTransform, Transform endTransform, float newMaxY = 5f, float newSpeed = 1f, float arrowRotOffsetZ = 0f)
    {
        startPos = startTransform.position;
        endPos = endTransform.position;
        maxY = newMaxY;
        speed = newSpeed;
        shooter = newShooter;
        renderTransform.rotation = Quaternion.Euler(0f, 0f, arrowRotOffsetZ);

        this.transform.position = startTransform.position;

        if (cancelToken != null)
        {
            cancelToken.Cancel();
        }
        else
        {
            cancelToken = new CancellationTokenSource();
        }

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

    private async Awaitable DeleteProc()
    {
        await Awaitable.WaitForSecondsAsync(1f);

        float alpha = 1f;
        float fadeSpeed = 5f;

        while (alpha > 0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;

            render.color = new Color(render.color.r, render.color.g, render.color.b, alpha);

            await Awaitable.NextFrameAsync();
        }

        alpha = 0f;
        Destroy(this.gameObject);
    }
}
