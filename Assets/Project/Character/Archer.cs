using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// 플레이어블 캐릭터를 제어하는 클래스
/// </summary>
public class Archer : SerializedMonoBehaviour
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
    public Dictionary<int, ObserverProperty<SO_Skill>> Skills { get => skills; }
    public bool IsInitialized { get => isInitialized; }

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
    [SerializeField] private Dictionary<int, ObserverProperty<SO_Skill>> skills = new Dictionary<int, ObserverProperty<SO_Skill>>();
    [SerializeField] private SO_Skill skill1;
    [SerializeField] private bool isInitialized = false;

    public void Initialize(Archer newTarget, ObserverProperty<int> newHP_Observer, float newSpeed)
    {
        target = newTarget;
        currentHP_Observer = newHP_Observer;
        maxHP = newHP_Observer.Value;
        speed = newSpeed;

        skills.Clear();
        skills.Add(0, new ObserverProperty<SO_Skill>(skill1));
        skills.Add(1, new ObserverProperty<SO_Skill>());
        skills.Add(2, new ObserverProperty<SO_Skill>());
        skills.Add(3, new ObserverProperty<SO_Skill>());
        skills.Add(4, new ObserverProperty<SO_Skill>());

        doBehavior = true;

        isInitialized = true;
    }

    public void TakeHit(int damage)
    {
        if (isInitialized == false)
        {
            return;
        }

        currentHP_Observer.Value -= damage;
    }

    public void Look(int xDirection)
    {
        if (isInitialized == false)
        {
            return;
        }

        var rot = xDirection == -1 ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);

        transform.localRotation = rot;
    }

    public void MoveFoward(int xDirection)
    {
        if (isInitialized == false)
        {
            return;
        }

        Look(xDirection);
        rigidBody.MovePosition(transform.position + Vector3.right * xDirection * speed * Time.deltaTime);
    }

    /// <summary>
    /// 애니메이션 클립 이벤트
    /// </summary>
    public void OnShoot()
    {
        if (isInitialized == false)
        {
            return;
        }

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
        if (isInitialized == false)
        {
            return;
        }

        state = AnimationEnum.Idle;
        animator.Play(AnimationHash.IDLE);
    }

    public void DoWalk()
    {
        if (isInitialized == false)
        {
            return;
        }

        state = AnimationEnum.Walk;
        animator.Play(AnimationHash.WALK);
    }

    public void DoAttack()
    {
        if (isInitialized == false)
        {
            return;
        }

        state = AnimationEnum.Attack;
        animator.Play(AnimationHash.ATTACK);
    }

    public void DoHurt()
    {
        if (isInitialized == false)
        {
            return;
        }

        state = AnimationEnum.Hurt;
        animator.Play(AnimationHash.HURT);
    }

    public void DoDie()
    {
        if (isInitialized == false)
        {
            return;
        }

        state = AnimationEnum.Die;
        animator.Play(AnimationHash.DIE);
    }

    public void DoVictory()
    {
        if (isInitialized == false)
        {
            return;
        }

        state = AnimationEnum.Victory;
        animator.Play(AnimationHash.VICTORY);
    }
}
