using _02.Scripts.Core;
using _02.Scripts.Data.Enemy;
using UnityEngine;

namespace _02.Scripts.Stage
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private WaveController _waveController;
        [SerializeField] private EnemyData _protoEnemyData;
        [SerializeField] private int _wavesPerChapter = 10;

        private int _currentChapter = 1;
        private int _currentWave = 1;

        public int CurrentChapter => _currentChapter;
        public int CurrentWave => _currentWave;

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
            _waveController.StartWave(_currentChapter, _currentWave, _protoEnemyData);
        }

        private void HandleWaveComplete(int chapter, int wave)
        {
            _currentWave++;

            if (_currentWave > _wavesPerChapter)
            {
                _currentChapter++;
                _currentWave = 1;
            }

            _waveController.StartWave(_currentChapter, _currentWave, _protoEnemyData);
        }
    }
}