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
        [SerializeField] private int _enemiesPerWave = 10;
        [SerializeField] private float _spawnInterval = 0.5f;
        [SerializeField] private float _waveStartDelay = 0.5f;

        private int _currentChapter;
        private int _currentWave;
        private int _killCount;
        private bool _isWaveActive;

        private void OnEnable()
        {
            GameEventBus.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEventBus.OnEnemyKilled -= HandleEnemyKilled;
        }

        public void StartWave(int chapter, int wave, EnemyData enemyData)
        {
            _currentChapter = chapter;
            _currentWave = wave;
            _killCount = 0;
            _isWaveActive = true;

            GameEventBus.RaiseWaveStart(chapter, wave);
            StartCoroutine(SpawnWaveRoutine(enemyData));
        }

        private IEnumerator SpawnWaveRoutine(EnemyData enemyData)
        {
            yield return new WaitForSeconds(_waveStartDelay);

            for (int i = 0; i < _enemiesPerWave; i++)
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

            if (_killCount >= _enemiesPerWave)
            {
                _isWaveActive = false;
                GameEventBus.RaiseWaveComplete(_currentChapter, _currentWave);
            }
        }
    }
}