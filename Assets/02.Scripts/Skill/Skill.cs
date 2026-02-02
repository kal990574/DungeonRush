using _02.Scripts.Battle;
using _02.Scripts.Character.Interfaces;
using _02.Scripts.Character.Player;
using _02.Scripts.Data.Skill;
using UnityEngine;

namespace _02.Scripts.Skill
{
    public class Skill
    {
        private readonly SkillData _data;
        private float _cooldownRemaining;
        private int _level = 1;

        public SkillData Data => _data;
        public int Level => _level;
        public float CooldownRemaining => _cooldownRemaining;
        public bool IsReady => _cooldownRemaining <= 0f;

        public Skill(SkillData data)
        {
            _data = data;
        }

        public void Execute(PlayerStats casterStats, IDamageable target)
        {
            if (!IsReady) return;

            float damage = CalculateDamage(casterStats);
            target.TakeDamage(damage, _data.damageType);

            _cooldownRemaining = _data.cooldown;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (_cooldownRemaining > 0f)
            {
                _cooldownRemaining -= deltaTime;
            }
        }

        public void LevelUp()
        {
            if (_level < _data.maxLevel)
            {
                _level++;
            }
        }

        private float CalculateDamage(PlayerStats stats)
        {
            float levelBonus = 1f + (_level - 1) * _data.damagePerLevel;
            float baseDamage = _data.baseDamage * levelBonus;
            return DamageCalculator.Calculate(baseDamage, stats);
        }
    }
}