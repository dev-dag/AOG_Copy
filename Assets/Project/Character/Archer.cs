using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 플레이어블 캐릭터를 제어하는 클래스
/// </summary>
public class Archer : MonoBehaviour
{
    public enum AnimationEnum
    {
        Idle,
        Walk,
        Attack,
        Hurt,
        Die
    }

    public struct AnimationHash
    {
        public static readonly int IDLE = Animator.StringToHash("idle");
        public static readonly int WALK = Animator.StringToHash("walk");
        public static readonly int ATTACK = Animator.StringToHash("attack");
        public static readonly int HURT = Animator.StringToHash("hurt");
        public static readonly int DIE = Animator.StringToHash("die");
    }

    public AnimationEnum State { get => state; }
    public Animator Animator { get => animator; }
    public float CurrentHP { get => currentHP; }
    public float MaxHP { get => maxHP; }
    public Rigidbody2D RigidBody { get => rigidBody; }
    public float Speed { get => speed; }

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private AnimationEnum state;
    [SerializeField] private float currentHP;
    [SerializeField] private float maxHP;
    [SerializeField] private float speed;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform handpoint;
    [SerializeField] private Transform endpoint;
    
    public void Look(int xDirection)
    {
        var rot = xDirection == -1 ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

        transform.localRotation = rot;
    }

    /// <summary>
    /// 애니메이션 클립 이벤트
    /// </summary>
    public void OnShoot()
    {
        var arrowInstance = GameObject.Instantiate(arrow).GetComponent<Arrow>();

        arrowInstance.Shoot(this.gameObject, handpoint, endpoint, newMaxY: 10f, newSpeed: 50f, arrowRotOffsetZ:45f);
    }

    public void DoIdle()
    {
        state = AnimationEnum.Idle;
        animator.Play(AnimationHash.IDLE);
    }

    public void DoWalk()
    {
        state = AnimationEnum.Walk;
        animator.Play(AnimationHash.WALK);
    }

    public void DoAttack()
    {
        state = AnimationEnum.Attack;
        animator.Play(AnimationHash.ATTACK);
    }

    public void DoHurt()
    {
        state = AnimationEnum.Hurt;
        animator.Play(AnimationHash.HURT);
    }

    public void DoDie()
    {
        state = AnimationEnum.Die;
        animator.Play(AnimationHash.DIE);
    }
}
