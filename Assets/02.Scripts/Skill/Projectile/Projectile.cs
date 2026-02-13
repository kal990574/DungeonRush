using System;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private float _damage;
    private float _speed;
    private Transform _target;
    private IDamageable _targetDamageable;
    private IObjectPool<Projectile> _pool;
    private Action<Vector3> _onHitEffect;
    private float _spawnTime;
    private const float Lifetime = 5f;
    private const float HitDistance = 0.2f;

    public void Initialize(float damage, float speed, Transform target, IObjectPool<Projectile> pool, Action<Vector3> onHitEffect)
    {
        _damage = damage;
        _speed = speed;
        _target = target;
        _targetDamageable = target.GetComponent<IDamageable>();
        _pool = pool;
        _onHitEffect = onHitEffect;
        _spawnTime = Time.time;
    }

    private void Update()
    {
        if (_target == null || !_targetDamageable.IsAlive || IsExpired())
        {
            _pool.Release(this);
            return;
        }

        MoveTowardTarget();

        if (HasReachedTarget())
        {
            _targetDamageable.TakeDamage(_damage);
            _onHitEffect?.Invoke(transform.position);
            _pool.Release(this);
        }
    }

    private void MoveTowardTarget()
    {
        Vector2 direction = (_target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * _speed * Time.deltaTime);
    }

    private bool HasReachedTarget()
    {
        return Vector2.Distance(transform.position, _target.position) < HitDistance;
    }

    private bool IsExpired()
    {
        return Time.time > _spawnTime + Lifetime;
    }
}