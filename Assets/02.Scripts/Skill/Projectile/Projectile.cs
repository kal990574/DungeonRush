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
    private Func<Transform, Transform> _retargetFunc;
    private Vector3 _lastTargetPosition;
    private float _spawnTime;
    private const float Lifetime = 5f;
    private const float HitDistance = 0.2f;

    public void Initialize(
        float damage, float speed, Transform target,
        IObjectPool<Projectile> pool, Action<Vector3> onHitEffect,
        Func<Transform, Transform> retargetFunc)
    {
        _damage = damage;
        _speed = speed;
        _pool = pool;
        _onHitEffect = onHitEffect;
        _retargetFunc = retargetFunc;
        _spawnTime = Time.time;
        SetTarget(target);
        RotateToward(target.position - transform.position);
    }

    private void Update()
    {
        if (IsExpired())
        {
            _pool.Release(this);
            return;
        }

        if (!HasValidTarget() && !TryRetarget())
        {
            MoveToLastPosition();
            return;
        }

        _lastTargetPosition = _target.position;
        MoveToward(_target.position);

        if (HasReached(_target.position))
        {
            _targetDamageable.TakeDamage(_damage);
            _onHitEffect?.Invoke(transform.position);
            _pool.Release(this);
        }
    }

    private void SetTarget(Transform target)
    {
        _target = target;
        _targetDamageable = target.GetComponent<IDamageable>();
        _lastTargetPosition = target.position;
    }

    private bool HasValidTarget()
    {
        return _target != null
            && _target.gameObject.activeInHierarchy
            && _targetDamageable.IsAlive;
    }

    private bool TryRetarget()
    {
        Transform newTarget = _retargetFunc?.Invoke(transform);
        if (newTarget == null) return false;

        SetTarget(newTarget);
        return true;
    }

    private void MoveToLastPosition()
    {
        MoveToward(_lastTargetPosition);

        if (HasReached(_lastTargetPosition))
        {
            _onHitEffect?.Invoke(transform.position);
            _pool.Release(this);
        }
    }

    private void MoveToward(Vector3 destination)
    {
        Vector2 direction = (destination - transform.position).normalized;
        transform.position += (Vector3)(direction * _speed * Time.deltaTime);
        RotateToward(direction);
    }

    private void RotateToward(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private bool HasReached(Vector3 position)
    {
        return Vector2.Distance(transform.position, position) < HitDistance;
    }

    private bool IsExpired()
    {
        return Time.time > _spawnTime + Lifetime;
    }
}