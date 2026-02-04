using UnityEngine;

public class AttackController : MonoBehaviour
{
    private float _attackDamage;
    private float _attackRange;
    private float _attackCooldown;
    private float _lastAttackTime = -999f;

    public float AttackRange => _attackRange;

    public void Initialize(float damage, float range, float cooldown)
    {
        _attackDamage = damage;
        _attackRange = range;
        _attackCooldown = cooldown;
    }

    public bool IsInRange(Transform target)
    {
        return Vector2.Distance(transform.position, target.position) <= _attackRange;
    }

    public bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _attackCooldown;
    }

    public bool TryAttack(Transform target)
    {
        if (!CanAttack() || !IsInRange(target)) return false;

        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null || !damageable.IsAlive) return false;

        _lastAttackTime = Time.time;
        damageable.TakeDamage(_attackDamage);
        return true;
    }
}