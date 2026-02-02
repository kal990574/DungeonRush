using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Character.Player
{
    public class PlayerController : CharacterBase
    {
        [SerializeField] private PlayerStats _playerStats;

        public PlayerStats Stats => _playerStats;

        protected override void Start()
        {
            _maxHp = _playerStats.MaxHp;
            _currentHp = _maxHp;
            base.Start();
        }

        public override void TakeDamage(float damage, DamageType damageType)
        {
            if (IsDead) return;

            _playerStats.TakeDamage(damage, damageType);
            _currentHp = _playerStats.CurrentHp;
            OnDamageTaken(damage);

            if (IsDead)
            {
                SetState(CharacterState.Dead);
                OnDeath();
            }
        }

        protected override void OnDamageTaken(float damage)
        {
            GameEventBus.RaisePlayerHpChanged(CurrentHp, MaxHp);
        }

        protected override void OnDeath()
        {
            GameEventBus.RaisePlayerDeath();
        }
    }
}
