using System;
using _02.Scripts.Character.Interfaces;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Character
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        private float _maxHp;
        private float _currentHp;

        public event Action<float> OnDamageTaken;
        public event Action OnDied;
        public event Action<float, float> OnHpChanged;

        public float CurrentHp => _currentHp;
        public float MaxHp => _maxHp;
        public bool IsDead => _currentHp <= 0;

        public void Initialize(float maxHp)
        {
            _maxHp = maxHp;
            _currentHp = maxHp;
            OnHpChanged?.Invoke(_currentHp, _maxHp);
        }

        public void TakeDamage(float damage, DamageType damageType)
        {
            if (IsDead) return;

            _currentHp = Mathf.Max(0, _currentHp - damage);
            OnDamageTaken?.Invoke(damage);
            OnHpChanged?.Invoke(_currentHp, _maxHp);

            if (IsDead)
            {
                OnDied?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;

            _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
            OnHpChanged?.Invoke(_currentHp, _maxHp);
        }
    }
}
