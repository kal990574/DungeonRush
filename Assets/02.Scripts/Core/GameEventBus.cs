using System;
using _02.Scripts.Character.Enemy;

namespace _02.Scripts.Core
{
    public static class GameEventBus
    {
        // state
        public static event Action<GameState> OnGameStateChanged;

        // player
        public static event Action<float, float> OnPlayerHpChanged;
        public static event Action<float, float> OnPlayerXpChanged;
        public static event Action<int> OnPlayerLevelUp;
        public static event Action OnPlayerDeath;

        // stage
        public static event Action<int, int> OnWaveStart;
        public static event Action<int, int> OnWaveComplete;

        // combat
        public static event Action<EnemyController> OnEnemySpawned;
        public static event Action<EnemyController, int> OnEnemyKilled;

        // Invoke
        public static void RaiseGameStateChanged(GameState gameState)
        {
            OnGameStateChanged?.Invoke(gameState);
        }

        public static void RaisePlayerHpChanged(float current, float max)
        {
            OnPlayerHpChanged?.Invoke(current, max);
        }

        public static void RaisePlayerXpChanged(float current, float max)
        {
            OnPlayerXpChanged?.Invoke(current, max);
        }

        public static void RaisePlayerLevelUp(int newLevel)
        {
            OnPlayerLevelUp?.Invoke(newLevel);
        }

        public static void RaisePlayerDeath()
        {
            OnPlayerDeath?.Invoke();
        }

        public static void RaiseWaveStart(int chapter, int wave)
        {
            OnWaveStart?.Invoke(chapter, wave);
        }

        public static void RaiseWaveComplete(int chapter, int wave)
        {
            OnWaveComplete?.Invoke(chapter, wave);
        }

        public static void RaiseEnemySpawned(EnemyController enemy)
        {
            OnEnemySpawned?.Invoke(enemy);
        }

        public static void RaiseEnemyKilled(EnemyController enemy, int xpReward)
        {
            OnEnemyKilled?.Invoke(enemy, xpReward);
        }

        // scene transition(?)
        public static void Clear()
        {
            OnGameStateChanged = null;
            OnPlayerHpChanged = null;
            OnPlayerXpChanged = null;
            OnPlayerLevelUp = null;
            OnPlayerDeath = null;
            OnWaveStart = null;
            OnWaveComplete = null;
            OnEnemySpawned = null;
            OnEnemyKilled = null;
        }
    }
}