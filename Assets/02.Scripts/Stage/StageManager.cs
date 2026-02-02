using _02.Scripts.Core;
using _02.Scripts.Data.Enemy;
using UnityEngine;

namespace _02.Scripts.Stage
{
    public class StageManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WaveController _waveController;

        [Header("Stage Settings")]
        [SerializeField] private int _wavesPerChapter = 10;
        [SerializeField] private int _enemiesPerWave = 10;

        [Header("Enemy Data")]
        [SerializeField] private EnemyData _normalEnemyData;
        [SerializeField] private EnemyData _bossEnemyData;

        [Header("Tempo")]
        [SerializeField] private float _normalWaveDelay = 0.5f;
        [SerializeField] private float _bossWaveDelay = 1.0f;

        private int _currentChapter = 1;
        private int _currentWave = 1;

        public int CurrentChapter => _currentChapter;
        public int CurrentWave => _currentWave;
        public bool IsBossWave => _currentWave == _wavesPerChapter;

        private void OnEnable()
        {
            GameEventBus.OnGameStateChanged += HandleGameStateChanged;
            GameEventBus.OnWaveComplete += HandleWaveComplete;
        }

        private void OnDisable()
        {
            GameEventBus.OnGameStateChanged -= HandleGameStateChanged;
            GameEventBus.OnWaveComplete -= HandleWaveComplete;
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Playing)
            {
                StartStage();
            }
        }

        private void StartStage()
        {
            _currentChapter = 1;
            _currentWave = 1;
            StartCurrentWave();
        }

        private void HandleWaveComplete(int chapter, int wave)
        {
            _currentWave++;

            if (_currentWave > _wavesPerChapter)
            {
                _currentChapter++;
                _currentWave = 1;
            }

            StartCurrentWave();
        }

        private void StartCurrentWave()
        {
            if (IsBossWave)
            {
                _waveController.StartWave(
                    _currentChapter, _currentWave,
                    _bossEnemyData, 1, _bossWaveDelay);
            }
            else
            {
                _waveController.StartWave(
                    _currentChapter, _currentWave,
                    _normalEnemyData, _enemiesPerWave, _normalWaveDelay);
            }
        }
    }
}