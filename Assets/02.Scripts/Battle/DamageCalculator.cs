using _02.Scripts.Character.Player;
using UnityEngine;

namespace _02.Scripts.Battle
{
    public static class DamageCalculator
    {
        public static float Calculate(float baseDamage, PlayerStats playerStats)
        {
            float damage = baseDamage * playerStats.DamageMultiplier;

            if (Random.value < playerStats.CritChance)
            {
                damage *= playerStats.CritDamage;
            }

            return Mathf.Max(1f, damage);
        }

        public static float Calculate(float baseDamage)
        {
            return Mathf.Max(1f, baseDamage);
        }
    }
}