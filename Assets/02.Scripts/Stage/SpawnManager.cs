using System.Collections;
using _02.Scripts.Character.Enemy;
using _02.Scripts.Core;
using _02.Scripts.Data.Enemy;
using UnityEngine;

namespace _02.Scripts.Stage
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private EnemyPool _enemyPool;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _spawnOffsetX = 12f;
        [SerializeField] private float _spawnRangeY = 4f;
        [SerializeField] private float _despawnDelay = 0.5f;

        private void OnEnable()
        {
            GameEventBus.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEventBus.OnEnemyKilled -= HandleEnemyKilled;
        }

        public void SpawnEnemy(EnemyData data)
        {
            EnemyController enemy = _enemyPool.Get();
            enemy.transform.position = GetSpawnPosition();
            enemy.Setup(data, _playerTransform);

            GameEventBus.RaiseEnemySpawned(enemy);
        }

        private Vector3 GetSpawnPosition()
        {
            float x = _playerTransform.position.x + _spawnOffsetX;
            float y = Random.Range(-_spawnRangeY, _spawnRangeY);
            return new Vector3(x, y, 0f);
        }

        private void HandleEnemyKilled(EnemyController enemy, int xp)
        {
            StartCoroutine(DelayedReturn(enemy));
        }

        private IEnumerator DelayedReturn(EnemyController enemy)
        {
            yield return new WaitForSeconds(_despawnDelay);
            _enemyPool.Return(enemy);
        }
    }
}
