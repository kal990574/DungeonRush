using System.Collections;
using _02.Scripts.Character.Enemy;
using _02.Scripts.Core;
using _02.Scripts.Data.Enemy;
using UnityEngine;

namespace _02.Scripts.Stage
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private SpawnManager _spawnManager;
        [SerializeField] private float _spawnInterval = 0.5f;

        private int _currentChapter;
        private int _currentWave;
        private int _killCount;
        private int _requiredKills;
        private bool _isWaveActive;

        private void OnEnable()
        {
            GameEventBus.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEventBus.OnEnemyKilled -= HandleEnemyKilled;
        }

        public void StartWave(int chapter, int wave, EnemyData enemyData, int enemyCount, float startDelay)
        {
            _currentChapter = chapter;
            _currentWave = wave;
            _killCount = 0;
            _requiredKills = enemyCount;
            _isWaveActive = true;

            GameEventBus.RaiseWaveStart(chapter, wave);
            StartCoroutine(SpawnWaveRoutine(enemyData, enemyCount, startDelay));
        }

        private IEnumerator SpawnWaveRoutine(EnemyData enemyData, int enemyCount, float startDelay)
        {
            yield return new WaitForSeconds(startDelay);

            for (int i = 0; i < enemyCount; i++)
            {
                if (!_isWaveActive) yield break;

                _spawnManager.SpawnEnemy(enemyData);
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void HandleEnemyKilled(EnemyController enemy, int xp)
        {
            if (!_isWaveActive) return;

            _killCount++;

            if (_killCount >= _requiredKills)
            {
                _isWaveActive = false;
                GameEventBus.RaiseWaveComplete(_currentChapter, _currentWave);
            }
        }
    }
}