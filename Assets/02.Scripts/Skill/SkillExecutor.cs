using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    [SerializeField] private SkillData _skillData;

    private float _lastExecuteTime = -999f;
    private ProjectilePool _projectilePool;

    public float AttackRange => _skillData.range;

    private void Start()
    {
        if (_skillData.skillType == SkillType.Projectile)
        {
            InitializePool();
        }
    }

    public bool IsInRange(Transform target)
    {
        return Vector2.Distance(transform.position, target.position) <= _skillData.range;
    }

    public bool CanExecute()
    {
        return Time.time >= _lastExecuteTime + _skillData.cooldown;
    }

    public bool TryExecute(Transform target)
    {
        if (!CanExecute() || !IsInRange(target)) return false;

        _lastExecuteTime = Time.time;

        switch (_skillData.skillType)
        {
            case SkillType.Melee:
                ExecuteMelee(target);
                break;
            case SkillType.Projectile:
                ExecuteProjectile(target);
                break;
        }

        return true;
    }

    private void ExecuteMelee(Transform target)
    {
        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            damageable.TakeDamage(_skillData.damage);
        }
    }

    private void ExecuteProjectile(Transform target)
    {
        Projectile projectile = _projectilePool.Get();
        projectile.transform.position = transform.position;
        projectile.Initialize(
            _skillData.damage, _skillData.projectileSpeed, target, _projectilePool);
    }

    private void InitializePool()
    {
        var poolObj = new GameObject($"Pool_{_skillData.skillName}");
        _projectilePool = poolObj.AddComponent<ProjectilePool>();
        _projectilePool.Initialize(_skillData.projectilePrefab);
    }
}