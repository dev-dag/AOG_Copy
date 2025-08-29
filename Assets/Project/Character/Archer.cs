using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
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
        Die,
        Victory,
    }

    public struct AnimationHash
    {
        public static readonly int IDLE = Animator.StringToHash("idle");
        public static readonly int WALK = Animator.StringToHash("walk");
        public static readonly int ATTACK = Animator.StringToHash("attack");
        public static readonly int HURT = Animator.StringToHash("hurt");
        public static readonly int VICTORY = Animator.StringToHash("victory");
        public static readonly int DIE = Animator.StringToHash("die");
    }

    public AnimationEnum State { get => state; }
    public Animator Animator { get => animator; }
    public float CurrentHP { get => currentHP_Observer.Value; }
    public float MaxHP { get => maxHP; }
    public Rigidbody2D RigidBody { get => rigidBody; }
    public float Speed { get => speed; }
    public bool DoBehavior { get => doBehavior; }
    public BoxCollider2D Collider { get => collider; }
    public Archer Target { get => target; }

    [SerializeField, Required] private Animator animator;
    [SerializeField, Required] private Rigidbody2D rigidBody;
    [SerializeField, Required] private BoxCollider2D collider;
    [SerializeField] private AnimationEnum state;
    [SerializeField] private ObserverProperty<int> currentHP_Observer;
    [SerializeField] private int maxHP;
    [SerializeField] private float speed;
    [SerializeField, Required] private GameObject arrow;
    [SerializeField, Required] private Transform handpoint;
    [SerializeField] private Archer target;
    [SerializeField] private bool doBehavior;

    public void Initialize(Archer newTarget, ObserverProperty<int> newHP_Observer, float newSpeed)
    {
        target = newTarget;
        currentHP_Observer = newHP_Observer;
        maxHP = newHP_Observer.Value;
        speed = newSpeed;
        currentHP_Observer.Value = 1000000000;

        doBehavior = true;
    }

    public void TakeHit(int damage)
    {
        currentHP_Observer.Value -= damage;
    }

    public void Look(int xDirection)
    {
        var rot = xDirection == -1 ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

        transform.localRotation = rot;
    }

    public void MoveFoward(int xDirection)
    {
        Look(xDirection);
        rigidBody.MovePosition(transform.position + Vector3.right * xDirection * speed * Time.deltaTime);
    }

    /// <summary>
    /// 애니메이션 클립 이벤트
    /// </summary>
    public void OnShoot()
    {
        return;

        var globalPool = GameManager.Instance.GameSceneControl.GlobalPool;

        if (globalPool.GetPool("Default Arrow") == null)
        {
            Func<object> create = () =>
            {
                var newArrow = GameObject.Instantiate(arrow, globalPool.transform).GetComponent<Arrow>();
                newArrow.Initialize(globalPool.GetPool("Default Arrow"));

                return newArrow;
            };

            Action<object> get = (instance) =>
            {
                var arrowInstnace = (Arrow)instance;
                arrowInstnace.gameObject.SetActive(true);
            };

            Action<object> release = (instance) =>
            {
                var arrowInstnace = (Arrow)instance;
                arrowInstnace.gameObject.SetActive(false);
            };

            globalPool.RegistPool<Arrow>("Default Arrow", create, get, release);
        }

        var arrowPool = globalPool.GetPool("Default Arrow");
        Arrow newArrow = (Arrow)arrowPool.Get();

        newArrow.Shoot(this, 100, handpoint, target.transform, newMaxY: 10f, newSpeed: 50f, arrowRotOffsetZ:45f);
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

    [Button("Shot")]
    public void DoAttackTest()
    {
        var globalPool = GameSceneControl.Instance.GlobalPool;

        if (globalPool.GetPool("Default Arrow") == null)
        {
            Func<object> create = () =>
            {
                var newArrow = GameObject.Instantiate(arrow, globalPool.transform).GetComponent<Arrow>();
                newArrow.Initialize(globalPool.GetPool("Default Arrow"));

                return newArrow;
            };

            Action<object> get = (instance) =>
            {
                var arrowInstnace = (Arrow)instance;
                arrowInstnace.gameObject.SetActive(true);
            };

            Action<object> release = (instance) =>
            {
                var arrowInstnace = (Arrow)instance;
                arrowInstnace.gameObject.SetActive(false);
            };

            globalPool.RegistPool<Arrow>("Default Arrow", create, get, release);
        }

        var arrowPool = globalPool.GetPool("Default Arrow");
        Arrow newArrow = (Arrow)arrowPool.Get();

        newArrow.Shoot(this, 100, handpoint, target.transform, newMaxY: 10f, newSpeed: 50f, arrowRotOffsetZ: 45f);
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

    public void DoVictory()
    {
        state = AnimationEnum.Victory;
        animator.Play(AnimationHash.VICTORY);
    }
}
