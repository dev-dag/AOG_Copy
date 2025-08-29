using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Behavior;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Pool;

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
        Skill,
    }

    public struct AnimationHash
    {
        // base layer
        public static readonly int IDLE = Animator.StringToHash("idle");
        public static readonly int WALK = Animator.StringToHash("walk");
        public static readonly int ATTACK = Animator.StringToHash("attack");
        public static readonly int HURT = Animator.StringToHash("hurt");
        public static readonly int VICTORY = Animator.StringToHash("victory");
        public static readonly int DIE = Animator.StringToHash("die");

        // condition layer
        public static readonly int CONDITION_NORMAL = Animator.StringToHash("condition_normal");
        public static readonly int CONDITION_FROZEN = Animator.StringToHash("condition_frozen");
        public static readonly int CONDITION_BURN = Animator.StringToHash("condition_burn");
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
    public Dictionary<int, ObserverProperty<float>> SkillCoolTimes { get => skillCoolTimes; }
    public ConditionControl ConditionControl { get => conditionControl; }

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
    [SerializeField] private Dictionary<int, ObserverProperty<float>> skillCoolTimes = new Dictionary<int, ObserverProperty<float>>();
    [SerializeField] private SO_Skill skill1;
    [SerializeField] private SO_Skill skill2;
    [SerializeField] private SO_Skill skill3;
    [SerializeField] private SO_Skill skill4;
    [SerializeField] private SO_Skill skill5;
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private ConditionControl conditionControl;
    [SerializeField] private ConditionEffector conditionAnimation;

    public void Initialize(Archer newTarget, ObserverProperty<int> newHP_Observer, float newSpeed)
    {
        conditionControl = new ConditionControl();
        conditionAnimation = new ConditionEffector(animator, conditionControl);
        target = newTarget;
        currentHP_Observer = newHP_Observer;
        maxHP = newHP_Observer.Value;
        speed = newSpeed;
        currentHP_Observer.Value = 1000000000;

        // 스킬 딕셔너리 초기화
        skills.Clear();
        skills.Add(0, new ObserverProperty<SO_Skill>(skill1));
        skills.Add(1, new ObserverProperty<SO_Skill>(skill2));
        skills.Add(2, new ObserverProperty<SO_Skill>(skill3));
        skills.Add(3, new ObserverProperty<SO_Skill>(skill4));
        skills.Add(4, new ObserverProperty<SO_Skill>(skill5));

        // 쿨타임 딕셔너리 초기화
        skillCoolTimes.Clear(); 
        skillCoolTimes.Add(0, new ObserverProperty<float>(0));
        skillCoolTimes.Add(1, new ObserverProperty<float>(0));
        skillCoolTimes.Add(2, new ObserverProperty<float>(0));
        skillCoolTimes.Add(3, new ObserverProperty<float>(0));
        skillCoolTimes.Add(4, new ObserverProperty<float>(0));

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

        if ((conditionControl.ConditionFlag & ConditionControl.ConditionEnum.Frozen) > 0) // 빙결 상태이상에 걸린 경우 이동속도 반감
        {
            rigidBody.MovePosition(transform.position + Vector3.right * xDirection * speed * 0.5f * Time.deltaTime);
        }
        else
        {
            rigidBody.MovePosition(transform.position + Vector3.right * xDirection * speed * Time.deltaTime);
        }
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

            globalPool.RegistPool("Default Arrow", create, get, release);
        }

        var arrowPool = globalPool.GetPool("Default Arrow");
        Arrow newArrow = (Arrow)arrowPool.Get();

        newArrow.Shoot(newShooter: this, newDamage: 100, startTransform: handpoint, endTransform: target.transform, newMaxY: 10f, newSpeed: 50f, arrowRotOffsetZ: 45f);
    }

    /// <summary>
    /// 애니메이션 클립 이벤트
    /// </summary>
    public void OnSkillShot(int skillID)
    {
        var skillData = GameSceneControl.Instance.SkillDatas[skillID];
        var globalPool = GameSceneControl.Instance.GlobalPool;

        // 화살 풀링
        string poolKey = $"Skill_Arrow_{skillData.Id}";

        if (globalPool.GetPool(poolKey) == null)
        {
            Func<object> create = () =>
            {
                var newArrow = GameObject.Instantiate(skillData.ArrowPrefab, globalPool.transform).GetComponent<Arrow>();
                newArrow.Initialize(globalPool.GetPool(poolKey));

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

            globalPool.RegistPool(poolKey, create, get, release);
        }

        var arrowPool = globalPool.GetPool(poolKey);
        Arrow newArrow = (Arrow)arrowPool.Get();

        // FX 풀링
        ObjectPool<object> fxPool = null;

        if (skillData.HitEffectPrefab != null)
        {
            string fxKey = $"FX_Skill_Hit_{skillData.Id}";

            if (globalPool.GetPool(fxKey) == null)
            {
                Func<object> create = () =>
                {
                    var newFX = GameObject.Instantiate(skillData.HitEffectPrefab, globalPool.transform).GetComponent<PoolingEffect>();

                    return newFX;
                };

                Action<object> get = (instance) =>
                {
                    var fxInstnace = (PoolingEffect)instance;
                    fxInstnace.gameObject.SetActive(true);
                };

                Action<object> release = (instance) =>
                {
                    var fxInstnace = (PoolingEffect)instance;
                    fxInstnace.gameObject.SetActive(false);
                };

                globalPool.RegistPool(fxKey, create, get, release);
            }

            fxPool = globalPool.GetPool(fxKey);
        }

        newArrow.Shoot(newShooter: this, newDamage: skillData.Damage, startTransform: handpoint, endTransform: target.transform, newFX_Pool: fxPool, newMaxY: skillData.MaxY, newSpeed: skillData.Speed, arrowRotOffsetZ: 45f);
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

    public void DoSkill(int index)
    {
        state = AnimationEnum.Skill;
        animator.Play(skills[index].Value.AnimatorStateName);
        skillCoolTimes[index].Value = Time.time + skills[index].Value.CoolTime;
    }
}
