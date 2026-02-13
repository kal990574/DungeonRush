using UnityEngine;

// 스킬 발사 로직만 담당, 쿨다운/사거리 판단 -> SkillSlot
public class SkillExecutor
{
    private readonly SkillData _data;
    private ProjectilePool _projectilePool;
    private EffectPool _effectPool;

    public SkillExecutor(SkillData data)
    {
        _data = data;
        InitializePools();
    }

    public void Execute(Transform owner, Transform target)
    {
        switch (_data.skillType)
        {
            case SkillType.Melee:
                ExecuteMelee(target);
                break;
            case SkillType.Projectile:
                ExecuteProjectile(owner, target);
                break;
        }
    }

    private void ExecuteMelee(Transform target)
    {
        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null || !damageable.IsAlive) return;

        damageable.TakeDamage(_data.damage);

        if (_effectPool != null)
        {
            _effectPool.Play(target.position);
        }
    }

    private void ExecuteProjectile(Transform owner, Transform target)
    {
        Projectile projectile = _projectilePool.Get();
        projectile.transform.position = owner.position;
        projectile.Initialize(
            _data.damage, _data.projectileSpeed, target,
            _projectilePool, _effectPool);
    }

    private void InitializePools()
    {
        if (_data.skillType == SkillType.Projectile && _data.projectilePrefab != null)
        {
            var poolObj = new GameObject($"Pool_{_data.skillName}");
            _projectilePool = poolObj.AddComponent<ProjectilePool>();
            _projectilePool.Initialize(_data.projectilePrefab);
        }

        if (_data.hitEffectPrefab != null)
        {
            var effectObj = new GameObject($"EffectPool_{_data.skillName}");
            _effectPool = effectObj.AddComponent<EffectPool>();
            _effectPool.Initialize(_data.hitEffectPrefab);
        }
    }
}