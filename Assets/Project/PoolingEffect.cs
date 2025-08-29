using UnityEngine;
using UnityEngine.Pool;

public class PoolingEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    private ObjectPool<object> pool;

    public void Initialize(ObjectPool<object> newPool)
    {
        pool = newPool;
        particle.Play();
    }

    private void Update()
    {
        if (particle.isPlaying == false)
        {
            pool.Release(this);
        }
    }
}