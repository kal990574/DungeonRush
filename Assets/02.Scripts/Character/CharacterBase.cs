using UnityEngine;

namespace _02.Scripts.Character
{
    public abstract class CharacterBase : MonoBehaviour
    {
        protected CharacterState _currentState = CharacterState.Running;
        protected SPUM_Prefabs _spumPrefabs;
        protected HealthComponent _health;

        public CharacterState CurrentState => _currentState;
        public HealthComponent Health => _health;
        public bool IsDead => _health.IsDead;

        protected virtual void Awake()
        {
            _spumPrefabs = GetComponentInChildren<SPUM_Prefabs>();
            _health = GetComponent<HealthComponent>();
        }

        protected virtual void Start()
        {
            _spumPrefabs.OverrideControllerInit();
            PlayAnimation(CharacterState.Running);
            _health.OnDied += HandleDeath;
        }

        protected virtual void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDeath;
            }
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

        private void HandleDeath()
        {
            SetState(CharacterState.Dead);
            OnDeath();
        }

        protected abstract void OnDeath();
    }
}
