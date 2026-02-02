using _02.Scripts.Character.Interfaces;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Character
{
    public abstract class CharacterBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] protected CharacterStats _stats;

        protected CharacterState _currentState = CharacterState.Running;
        protected SPUM_Prefabs _spumPrefabs;

        // IDamageable.
        public float CurrentHp => _stats.CurrentHp;
        public float MaxHp => _stats.MaxHp;
        public bool IsDead => _stats.IsDead;

        public CharacterState CurrentState => _currentState;

        protected virtual void Awake()
        {
            _spumPrefabs = GetComponentInChildren<SPUM_Prefabs>();
        }

        protected virtual void Start()
        {
            _stats.Initialize();
            _spumPrefabs.OverrideControllerInit();
            PlayAnimation(CharacterState.Running);
        }

        public void TakeDamage(float damage, DamageType damageType)
        {
            if (IsDead) return;

            _stats.TakeDamage(damage, damageType);
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
            _stats.Heal(amount);
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