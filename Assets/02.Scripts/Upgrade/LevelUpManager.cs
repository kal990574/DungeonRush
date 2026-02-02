using _02.Scripts.Character.Enemy;
using _02.Scripts.Character.Player;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Upgrade
{
    public class LevelUpManager : MonoBehaviour
    {
        [SerializeField] private PlayerStats _playerStats;

        private int _currentLevel = 1;
        private float _currentXp;
        private float _xpToNextLevel;

        public int CurrentLevel => _currentLevel;
        public float CurrentXp => _currentXp;
        public float XpToNextLevel => _xpToNextLevel;

        private void Start()
        {
            _xpToNextLevel = CalculateXpThreshold(_currentLevel);
            GameEventBus.RaisePlayerXpChanged(_currentXp, _xpToNextLevel);
        }

        private void OnEnable()
        {
            GameEventBus.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEventBus.OnEnemyKilled -= HandleEnemyKilled;
        }

        private void HandleEnemyKilled(EnemyController enemy, int xpReward)
        {
            float xpGain = xpReward * _playerStats.XpMultiplier;
            AddXp(xpGain);
        }

        private void AddXp(float amount)
        {
            _currentXp += amount;

            while (_currentXp >= _xpToNextLevel)
            {
                _currentXp -= _xpToNextLevel;
                LevelUp();
            }

            GameEventBus.RaisePlayerXpChanged(_currentXp, _xpToNextLevel);
        }

        private void LevelUp()
        {
            _currentLevel++;
            _xpToNextLevel = CalculateXpThreshold(_currentLevel);
            GameEventBus.RaisePlayerLevelUp(_currentLevel);
            GameManager.Instance.ChangeState(GameState.LevelUp);
        }

        private float CalculateXpThreshold(int level)
        {
            GameConfig config = GameManager.Instance.GameConfig;
            return config.BaseXpRequired * Mathf.Pow(config.XpMultiplierPerLevel, level - 1);
        }
    }
}