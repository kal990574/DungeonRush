using _02.Scripts.Character.Interfaces;
using _02.Scripts.Data.Player;

namespace _02.Scripts.Character.Player
{
    public class PlayerCombatStats : ICombatStats
    {
        private float _damageMultiplier;
        private float _critChance;
        private float _critDamage;
        private float _xpMultiplier;

        public float DamageMultiplier => _damageMultiplier;
        public float CritChance => _critChance;
        public float CritDamage => _critDamage;
        public float XpMultiplier => _xpMultiplier;

        public PlayerCombatStats(PlayerData data)
        {
            _damageMultiplier = data.DamageMultiplier;
            _critChance = data.CritChance;
            _critDamage = data.CritDamage;
            _xpMultiplier = data.XpMultiplier;
        }
    }
}
