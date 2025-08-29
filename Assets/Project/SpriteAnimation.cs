using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
    public List<Sprite> Sprites { get => sprites; }
    public bool IsPlaying { get => isPlaying; }

    public bool playOnAwake = false;

    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private SpriteRenderer render;
    [SerializeField] private float frameDeltaTime;

    private bool isPlaying = false;
    private int index = 0;
    private CancellationTokenSource canceler;

    private void Reset()
    {
        render = GetComponent<SpriteRenderer>();
        frameDeltaTime = 0.1f;
    }

    private void OnEnable()
    {
        if (playOnAwake)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        isPlaying = false;
    }

    public void Play()
    {
        index = 0;

        if (canceler != null)
        {
            canceler.Cancel();
        }

        canceler = new CancellationTokenSource();

        Animation(canceler.Token);
    }

    private async Awaitable Animation(CancellationToken ct)
    {
        isPlaying = true;

        while (isPlaying)
        {
            ct.ThrowIfCancellationRequested();
            render.sprite = sprites[index];
            index = ++index % sprites.Count;

            await Awaitable.WaitForSecondsAsync(frameDeltaTime);
        }
    }
}