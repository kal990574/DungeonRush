#if UNITY_EDITOR
using UnityEngine;
using DungeonRush.Core;

namespace DungeonRush.Debug
{
    public class HUDTestController : MonoBehaviour
    {
        [Header("HP Settings")]
        [SerializeField] private float _maxHP = 100f;
        [SerializeField] private float _damageAmount = 15f;

        [Header("XP Settings")]
        [SerializeField] private float _xpGainAmount = 25f;
        [SerializeField] private float _requiredXP = 100f;

        [Header("Gold Settings")]
        [SerializeField] private int _goldGainAmount = 50;

        private float _currentHP;
        private float _currentXP;
        private int _currentGold;
        private int _currentLevel = 1;
        private int _currentChapter = 1;
        private int _currentWave = 1;

        private void Start()
        {
            _currentHP = _maxHP;
            GameEventBus.PublishPlayerHPChanged(_currentHP, _maxHP);
            GameEventBus.PublishPlayerXPChanged(0f, _requiredXP);
            GameEventBus.PublishGoldChanged(0, 0);
            GameEventBus.PublishWaveStart(_currentChapter, _currentWave);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FullHeal();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TakeDamage();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GainXP();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GainGold();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                AdvanceStage();
            }
        }

        private void FullHeal()
        {
            _currentHP = _maxHP;
            GameEventBus.PublishPlayerHPChanged(_currentHP, _maxHP);
            GameEventBus.PublishPlayerHealed(_maxHP);
        }

        private void TakeDamage()
        {
            _currentHP = Mathf.Max(0f, _currentHP - _damageAmount);
            GameEventBus.PublishPlayerHPChanged(_currentHP, _maxHP);
            GameEventBus.PublishPlayerDamaged(_damageAmount);
        }

        private void GainXP()
        {
            _currentXP += _xpGainAmount;

            if (_currentXP >= _requiredXP)
            {
                _currentXP -= _requiredXP;
                _currentLevel++;
                GameEventBus.PublishPlayerLevelUp(_currentLevel);
            }

            GameEventBus.PublishPlayerXPChanged(_currentXP, _requiredXP);
        }

        private void GainGold()
        {
            _currentGold += _goldGainAmount;
            GameEventBus.PublishGoldChanged(_currentGold, _goldGainAmount);
        }

        private void AdvanceStage()
        {
            _currentWave++;

            if (_currentWave > 10)
            {
                _currentWave = 1;
                _currentChapter++;
                GameEventBus.PublishChapterStart(_currentChapter);
            }

            if (_currentWave == 10)
            {
                GameEventBus.PublishBossWaveStart(_currentChapter, _currentWave);
            }
            else
            {
                GameEventBus.PublishWaveStart(_currentChapter, _currentWave);
            }
        }
    }
}
#endif
