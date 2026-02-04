using _02.Scripts.Character.Interfaces;
using UnityEngine;

namespace _02.Scripts.Battle
{
    public static class DamageCalculator
    {
        public static float Calculate(float baseDamage, ICombatStats stats)
        {
            float damage = baseDamage * stats.DamageMultiplier;

            if (Random.value < stats.CritChance)
            {
                damage *= stats.CritDamage;
            }

            return Mathf.Max(1f, damage);
        }

        public static float Calculate(float baseDamage)
        {
            return Mathf.Max(1f, baseDamage);
        }
    }
}
