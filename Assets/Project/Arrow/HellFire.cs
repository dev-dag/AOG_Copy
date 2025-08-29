using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HellFire : MonoBehaviour
{
    public event Action onExitEvent;
    public event Action<Archer> onHitEvent;

    [SerializeField] private GameObject indicateInstance;
    [SerializeField] private Vector2 indicateOffset;

    [Space(15f)]
    [SerializeField] private GameObject meteoInstance;
    [SerializeField] private Vector2 metheOffset;
    [SerializeField] private float fallSpeed;

    [Space(15f)]
    [SerializeField] private GameObject hitInstance;
    [SerializeField] private Vector2 hitOffset;

    [Space(15f)]
    [SerializeField] private Vector2 origin;
    private CancellationTokenSource canceler;

    [Space(15f)]
    [SerializeField] private Vector2 hitRange;

    private void Awake()
    {
        this.transform.position = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(LayerDefine.PLATFORM))
        {
            return;
        }

        canceler.Cancel();
        meteoInstance.SetActive(false);

        hitInstance.SetActive(true);

        var hitParticle = hitInstance.GetComponent<ParticleSystem>();
        hitParticle.Play();

        var hit = Physics2D.BoxCast(origin, hitRange, 0f, Vector2.zero, 0f, LayerMask.GetMask(LayerDefine.ARCHER));
        if (hit.collider != null)
        {
            var hitArcher = hit.collider.gameObject.GetComponentInParent<Archer>();
            if (hitArcher != null)
            {
                onHitEvent?.Invoke(hitArcher);
            }
        }

        WaitForParticleEnd(hitParticle, () => onExitEvent?.Invoke());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(origin, hitRange);
    }

    public void Initialize(Vector2 newOrigin)
    {
        onExitEvent = null;
        onHitEvent = null;

        origin = newOrigin;

        indicateInstance.transform.position = origin + indicateOffset;
        indicateInstance.SetActive(false);

        meteoInstance.transform.position = origin + metheOffset;
        meteoInstance.SetActive(false);

        hitInstance.transform.position = origin + hitOffset;
        hitInstance.SetActive(false);

        if (canceler != null)
        {
            canceler.Cancel();
        }

        canceler = new CancellationTokenSource();

        Flow(canceler.Token);
    }

    private async Awaitable Flow(CancellationToken ct)
    {
        indicateInstance.gameObject.SetActive(true);
        var indicateParticle = indicateInstance.GetComponent<ParticleSystem>();
        indicateParticle.Play();

        await Awaitable.WaitForSecondsAsync(1f);
        indicateInstance.SetActive(false);

        meteoInstance.gameObject.SetActive(true);
        var meteoParticle = meteoInstance.GetComponent<ParticleSystem>();
        meteoParticle.Play();

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            meteoInstance.transform.position = meteoInstance.transform.position + Vector3.down * fallSpeed * Time.deltaTime;

            await Awaitable.NextFrameAsync();
        }
    }

    private async Awaitable WaitForParticleEnd(ParticleSystem particle, Action callback)
    {
        while (particle.isPlaying)
        {
            await Awaitable.NextFrameAsync();
        }

        callback?.Invoke();
    }
}