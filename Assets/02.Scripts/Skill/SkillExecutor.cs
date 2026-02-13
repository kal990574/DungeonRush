using System;
using UnityEngine;
using UnityEngine.Pool;

// 스킬 발사 로직만 담당, 쿨다운/사거리 판단 -> SkillSlot
public class SkillExecutor
{
    private readonly SkillData _data;
    private IObjectPool<Projectile> _projectilePool;
    private IObjectPool<EffectAutoReturn> _effectPool;

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
        PlayEffect(target.position);
    }

    private void ExecuteProjectile(Transform owner, Transform target)
    {
        Vector2 direction = (target.position - owner.position).normalized;
        Projectile projectile = _projectilePool.Get();
        projectile.transform.position = owner.position;
        projectile.Initialize(
            _data.damage, _data.projectileSpeed, _data.range, direction,
            _projectilePool, PlayEffect);
    }

    private void PlayEffect(Vector3 position)
    {
        if (_effectPool == null) return;

        EffectAutoReturn effect = _effectPool.Get();
        effect.transform.position = position;
    }

    private void InitializePools()
    {
        if (_data.skillType == SkillType.Projectile && _data.projectilePrefab != null)
        {
            Transform projectileParent = new GameObject($"Pool_{_data.skillName}").transform;
            _projectilePool = new ObjectPool<Projectile>(
                createFunc: () => UnityEngine.Object.Instantiate(_data.projectilePrefab, projectileParent),
                actionOnGet: p => p.gameObject.SetActive(true),
                actionOnRelease: p => p.gameObject.SetActive(false),
                actionOnDestroy: p => UnityEngine.Object.Destroy(p.gameObject),
                defaultCapacity: 10,
                maxSize: 30);
        }

        if (_data.hitEffectPrefab != null)
        {
            Transform effectParent = new GameObject($"EffectPool_{_data.skillName}").transform;
            IObjectPool<EffectAutoReturn> pool = null;
            pool = new ObjectPool<EffectAutoReturn>(
                createFunc: () =>
                {
                    EffectAutoReturn effect = UnityEngine.Object.Instantiate(_data.hitEffectPrefab, effectParent);
                    effect.OnFinished += e => pool.Release(e);
                    return effect;
                },
                actionOnGet: e => e.gameObject.SetActive(true),
                actionOnRelease: e => e.gameObject.SetActive(false),
                actionOnDestroy: e => UnityEngine.Object.Destroy(e.gameObject),
                defaultCapacity: 5,
                maxSize: 20);
            _effectPool = pool;
        }
    }
}