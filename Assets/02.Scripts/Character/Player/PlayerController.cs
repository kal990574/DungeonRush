using _02.Scripts.Character.Interfaces;
using _02.Scripts.Core;
using _02.Scripts.Data.Player;
using UnityEngine;

namespace _02.Scripts.Character.Player
{
    public class PlayerController : CharacterBase
    {
        [SerializeField] private PlayerData _playerData;

        private PlayerCombatStats _combatStats;

        public ICombatStats CombatStats => _combatStats;
        public float XpMultiplier => _combatStats.XpMultiplier;

        protected override void Awake()
        {
            base.Awake();
            _combatStats = new PlayerCombatStats(_playerData);
        }

        protected override void Start()
        {
            _health.Initialize(_playerData.MaxHp);
            _health.OnHpChanged += HandleHpChanged;
            base.Start();
        }

        protected override void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnHpChanged -= HandleHpChanged;
            }
            base.OnDestroy();
        }

        protected override void OnDeath()
        {
            GameEventBus.RaisePlayerDeath();
        }

        private void HandleHpChanged(float current, float max)
        {
            GameEventBus.RaisePlayerHpChanged(current, max);
        }
    }
}
