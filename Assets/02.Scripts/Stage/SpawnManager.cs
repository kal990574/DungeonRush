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
        [SerializeField] private float _spawnDistance = 8f;
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
            enemy.transform.position = GetRandomSpawnPosition();
            enemy.Setup(data, _playerTransform);

            GameEventBus.RaiseEnemySpawned(enemy);
        }

        private Vector3 GetRandomSpawnPosition()
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float x = Mathf.Cos(angle) * _spawnDistance;
            float y = Mathf.Sin(angle) * _spawnDistance;

            return _playerTransform.position + new Vector3(x, y, 0f);
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
