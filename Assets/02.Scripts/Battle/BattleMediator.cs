using System.Collections.Generic;
using _02.Scripts.Character.Enemy;
using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Battle
{
    public class BattleMediator : MonoBehaviour
    {
        private readonly List<EnemyController> _activeEnemies = new List<EnemyController>();

        public IReadOnlyList<EnemyController> ActiveEnemies => _activeEnemies;

        private void OnEnable()
        {
            GameEventBus.OnEnemySpawned += RegisterEnemy;
            GameEventBus.OnEnemyKilled += UnregisterEnemy;
        }

        private void OnDisable()
        {
            GameEventBus.OnEnemySpawned -= RegisterEnemy;
            GameEventBus.OnEnemyKilled -= UnregisterEnemy;
        }

        public EnemyController FindNearestEnemy(Vector3 position)
        {
            EnemyController nearest = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in _activeEnemies)
            {
                if (enemy.IsDead) continue;

                float distance = Vector2.Distance(position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        private void RegisterEnemy(EnemyController enemy)
        {
            _activeEnemies.Add(enemy);
        }

        private void UnregisterEnemy(EnemyController enemy, int xp)
        {
            _activeEnemies.Remove(enemy);
        }
    }
}