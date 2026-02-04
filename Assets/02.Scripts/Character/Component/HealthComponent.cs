using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
    private float _currentHp;
    private float _maxHp;

    public bool IsAlive => _currentHp > 0;
    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;

    public event Action OnDied;

    public void Initialize(float maxHp)
    {
        _maxHp = maxHp;
        _currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnDied?.Invoke();
        }
    }
}