using System;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HellFire : MonoBehaviour
{
    public event Action onExitEvent;

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

        meteoInstance.SetActive(false);

        hitInstance.SetActive(true);

        var hitParticle = hitInstance.GetComponent<ParticleSystem>();
        hitParticle.Play();

        WaitForParticleEnd(hitParticle, () => onExitEvent?.Invoke());
    }

    public void Initialize(Vector2 newOrigin)
    {
        origin = newOrigin;

        indicateInstance.transform.position = origin + indicateOffset;
        indicateInstance.SetActive(false);

        meteoInstance.transform.position = origin + metheOffset;
        meteoInstance.SetActive(false);

        hitInstance.transform.position = origin + hitOffset;
        hitInstance.SetActive(false);

        if (canceler == null)
        {
            canceler = new CancellationTokenSource();
        }

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