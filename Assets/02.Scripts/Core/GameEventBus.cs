using System;
using _02.Scripts.Character.Interfaces;

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

        // combat
        public static event Action<IPoolable> OnEnemySpawned;
        public static event Action<IPoolable, int> OnEnemyKilled;

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

        public static void RaiseEnemySpawned(IPoolable enemy)
        {
            OnEnemySpawned?.Invoke(enemy);
        }

        public static void RaiseEnemyKilled(IPoolable enemy, int xpReward)
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
            OnEnemySpawned = null;
            OnEnemyKilled = null;
        }
    }
}