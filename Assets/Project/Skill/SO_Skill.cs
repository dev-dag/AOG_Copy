using UnityEngine;

[CreateAssetMenu(fileName = "Skill_", menuName = "SO/Skill")]
public class SO_Skill : ScriptableObject
{
    public Arrow ArrowPrefab { get => arrowPrefab; }
    public int Id { get => id; }
    public float Speed { get => speed; }
    public string Description { get => description; }
    public float MaxY { get => maxY; }
    public Sprite Icon { get => icon; }
    public float CoolTime { get => coolTime; }
    public string AnimatorStateName { get => animatorStateName; }
    public int Damage { get => damage; }
    public PoolingEffect HitEffectPrefab { get => hitEffectPrefab; }

    [SerializeField] Arrow arrowPrefab;
    [SerializeField] PoolingEffect hitEffectPrefab;
    [SerializeField] private string animatorStateName;
    [SerializeField] private int id;
    [SerializeField] private float speed;
    [SerializeField] private string description;
    [SerializeField] private float maxY;
    [SerializeField] private float coolTime;
    [SerializeField] private int damage;
    [SerializeField] private Sprite icon;
}