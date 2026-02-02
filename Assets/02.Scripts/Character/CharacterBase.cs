using _02.Scripts.Character.Interfaces;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Character
{
    public abstract class CharacterBase : MonoBehaviour, IDamageable
    {
        protected float _currentHp;
        protected float _maxHp;
        protected CharacterState _currentState = CharacterState.Running;
        protected SPUM_Prefabs _spumPrefabs;

        // IDamageable.
        public float CurrentHp => _currentHp;
        public float MaxHp => _maxHp;
        public bool IsDead => _currentHp <= 0;

        public CharacterState CurrentState => _currentState;

        protected virtual void Awake()
        {
            _spumPrefabs = GetComponentInChildren<SPUM_Prefabs>();
        }

        protected virtual void Start()
        {
            _spumPrefabs.OverrideControllerInit();
            PlayAnimation(CharacterState.Running);
        }

        public virtual void TakeDamage(float damage, DamageType damageType)
        {
            if (IsDead) return;

            _currentHp = Mathf.Max(0, _currentHp - damage);
            OnDamageTaken(damage);

            if (IsDead)
            {
                SetState(CharacterState.Dead);
                OnDeath();
            }
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
        }

        protected void SetState(CharacterState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;
            PlayAnimation(newState);
        }

        protected void PlayAnimation(CharacterState state)
        {
            var spumState = state switch
            {
                CharacterState.Running => PlayerState.MOVE,
                CharacterState.Attacking => PlayerState.ATTACK,
                CharacterState.Dead => PlayerState.DEATH,
                _ => PlayerState.MOVE,
            };
            _spumPrefabs.PlayAnimation(spumState, 0);
        }

        protected abstract void OnDamageTaken(float damage);
        protected abstract void OnDeath();
    }
}
