using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 파티클 시스템의 작동이 끝나면 자동으로 풀로 돌아가게 제어하는 클래스
/// </summary>
public class PoolingEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private Vector2 offset;

    private ObjectPool<object> pool;

    public void Initialize(ObjectPool<object> newPool)
    {
        pool = newPool;
        transform.position = transform.position + (Vector3)offset;
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