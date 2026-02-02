using _02.Scripts.Character.Enemy;
using _02.Scripts.Character.Player;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Battle
{
    public class AutoBattleController : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private BattleMediator _battleMediator;
        [SerializeField] private float _targetSearchInterval = 0.2f;

        private float _searchTimer;
        private EnemyController _currentTarget;
        private bool _isActive;

        private void OnEnable()
        {
            GameEventBus.OnGameStateChanged += HandleStateChange;
        }

        private void OnDisable()
        {
            GameEventBus.OnGameStateChanged -= HandleStateChange;
        }

        private void Update()
        {
            if (!_isActive || _player.IsDead) return;

            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0f)
            {
                _searchTimer = _targetSearchInterval;
                _currentTarget = _battleMediator.FindNearestEnemy(_player.transform.position);
            }

            // TODO: Phase 4에서 스킬 시스템 연동.
        }

        private void HandleStateChange(GameState state)
        {
            _isActive = state == GameState.Playing;
        }
    }
}