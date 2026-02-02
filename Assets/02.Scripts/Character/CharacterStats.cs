using System;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Character
{
    [Serializable]
    public class CharacterStats
    {
      [Header("Survival - 생존")]
      [SerializeField] private float _maxHp = 100f;
      [SerializeField] private float _currentHp;
      [SerializeField] private float _hpRegen = 0f;
      [SerializeField] private float _overHeal = 0f;
      [SerializeField] private float _shield = 0f;
      [SerializeField] private float _evasion = 0f;
      [SerializeField] private float _lifeSteal = 0f;
      [SerializeField] private float _physicalDamageReduction = 0f;
      [SerializeField] private float _magicDamageReduction = 0f;
      [SerializeField] private float _knockbackResist = 0f;
      [SerializeField] private float _statusResist = 0f;

      [Header("Attack - 공격")]
      [SerializeField] private float _damageMultiplier = 1f;
      [SerializeField] private float _critChance = 0f;
      [SerializeField] private float _critDamage = 1.5f;
      [SerializeField] private float _attackSpeed = 1f;
      [SerializeField] private float _bossDamageMultiplier = 1f;
      [SerializeField] private float _spellPower = 0f;
      [SerializeField] private float _dotDamageMultiplier = 1f;

      [Header("Skill - 스킬/시전")]
      [SerializeField] private float _castSpeed = 1f;
      [SerializeField] private float _cooldownReduction = 0f;
      [SerializeField] private float _durationMultiplier = 1f;
      [SerializeField] private float _skillTickMultiplier = 1f;
      [SerializeField] private float _multiCastChance = 0f;

      [Header("Projectile - 투사체")]
      [SerializeField] private int _projectileCount = 1;
      [SerializeField] private int _bounceCount = 0;
      [SerializeField] private float _projectileSpeed = 1f;
      [SerializeField] private int _pierceCount = 0;
      [SerializeField] private int _chainCount = 0;
      [SerializeField] private float _projectileSize = 1f;

      [Header("Summon - 소환수")]
      [SerializeField] private float _summonDamageMultiplier = 1f;
      [SerializeField] private float _summonHpMultiplier = 1f;
      [SerializeField] private float _summonAttackSpeed = 1f;

      [Header("Utility - 유틸리티")]
      [SerializeField] private float _luck = 0f;
      [SerializeField] private float _xpMultiplier = 1f;
      [SerializeField] private float _goldMultiplier = 1f;
      [SerializeField] private float _pickupRange = 1f;
      [SerializeField] private float _moveSpeed = 3f;
      
      // 생존
      public float MaxHp => _maxHp;
      public float CurrentHp => _currentHp;
      public float EffectiveMaxHp => _maxHp + _overHeal;
      public float HpRegen => _hpRegen;
      public float OverHeal => _overHeal;
      public float Shield => _shield;
      public float Evasion => _evasion;
      public float LifeSteal => _lifeSteal;
      public float PhysicalDamageReduction => _physicalDamageReduction;
      public float MagicDamageReduction => _magicDamageReduction;
      public float KnockbackResist => _knockbackResist;
      public float StatusResist => _statusResist;

      // 공격
      public float DamageMultiplier => _damageMultiplier;
      public float CritChance => _critChance;
      public float CritDamage => _critDamage;
      public float AttackSpeed => _attackSpeed;
      public float BossDamageMultiplier => _bossDamageMultiplier;
      public float SpellPower => _spellPower;
      public float DotDamageMultiplier => _dotDamageMultiplier;

      // 스킬/시전
      public float CastSpeed => _castSpeed;
      public float CooldownReduction => Mathf.Clamp(_cooldownReduction, 0f, 0.8f);
      public float DurationMultiplier => _durationMultiplier;
      public float SkillTickMultiplier => _skillTickMultiplier;
      public float MultiCastChance => _multiCastChance;

      // 투사체
      public int ProjectileCount => _projectileCount;
      public int BounceCount => _bounceCount;
      public float ProjectileSpeed => _projectileSpeed;
      public int PierceCount => _pierceCount;
      public int ChainCount => _chainCount;
      public float ProjectileSize => _projectileSize;

      // 소환수
      public float SummonDamageMultiplier => _summonDamageMultiplier;
      public float SummonHpMultiplier => _summonHpMultiplier;
      public float SummonAttackSpeed => _summonAttackSpeed;

      // 유틸
      public float Luck => _luck;
      public float XpMultiplier => _xpMultiplier;
      public float GoldMultiplier => _goldMultiplier;
      public float PickupRange => _pickupRange;
      public float MoveSpeed => _moveSpeed;

      public bool IsDead => _currentHp <= 0;
      
      public void Initialize()
      {
          _currentHp = _maxHp;
      }
      
      public void TakeDamage(float damage, DamageType damageType)
      {
          // True 데미지는 감소 무시.
          if (damageType != DamageType.True)
          {
              float reduction = damageType == DamageType.Physical
                  ? _physicalDamageReduction
                  : _magicDamageReduction;

              damage *= 1f - Mathf.Clamp01(reduction);
          }

          // 실드 흡수.
          if (_shield > 0)
          {
              float absorbed = Mathf.Min(_shield, damage);
              _shield -= absorbed;
              damage -= absorbed;
          }

          _currentHp = Mathf.Max(0, _currentHp - damage);
      }
      
      public void Heal(float amount)
      {
          _currentHp = Mathf.Min(_currentHp + amount, EffectiveMaxHp);
      }
      
    }
}