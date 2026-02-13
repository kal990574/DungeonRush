using System;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;

    private float _damage;
    private float _speed;
    private float _maxRange;
    private Vector2 _direction;
    private Vector3 _spawnPosition;
    private IObjectPool<Projectile> _pool;
    private Action<Vector3> _onHitEffect;

    public void Initialize(
        float damage, float speed, float maxRange, Vector2 direction,
        IObjectPool<Projectile> pool, Action<Vector3> onHitEffect)
    {
        _damage = damage;
        _speed = speed;
        _maxRange = maxRange;
        _direction = direction.normalized;
        _pool = pool;
        _onHitEffect = onHitEffect;
        _spawnPosition = transform.position;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));

        float distanceSqr = (transform.position - _spawnPosition).sqrMagnitude;
        if (distanceSqr > _maxRange * _maxRange)
        {
            _pool.Release(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((_targetLayer.value & (1 << other.gameObject.layer)) == 0) return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null || !damageable.IsAlive) return;

        damageable.TakeDamage(_damage);
        _onHitEffect?.Invoke(other.transform.position);
        _pool.Release(this);
    }
}