using _02.Scripts.Character.Enemy;
using _02.Scripts.Character.Interfaces;
using _02.Scripts.Character.Player;
using _02.Scripts.Core;
using _02.Scripts.Data.Skill;
using _02.Scripts.Skill;
using UnityEngine;

namespace _02.Scripts.Battle
{
    public class AutoBattleController : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private BattleMediator _battleMediator;
        [SerializeField] private float _targetSearchInterval = 0.2f;

        [Header("Initial Skills")]
        [SerializeField] private SkillData[] _startingSkills;

        private float _searchTimer;
        private EnemyController _currentTarget;
        private SkillSlotManager _skillSlots;
        private bool _isActive;

        private void Awake()
        {
            _skillSlots = new SkillSlotManager();
        }

        private void Start()
        {
            EquipStartingSkills();
        }

        private void OnEnable()
        {
            GameEventBus.OnGameStateChanged += HandleStateChange;
        }

        private void OnDisable()
        {
            GameEventBus.OnGameStateChanged -= HandleStateChange;
        }

        private void Update()
        {
            if (!_isActive || _player.IsDead) return;

            _skillSlots.UpdateAllCooldowns(Time.deltaTime);

            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0f)
            {
                _searchTimer = _targetSearchInterval;
                _currentTarget = _battleMediator.FindNearestEnemy(_player.transform.position);
            }

            if (_currentTarget == null || _currentTarget.IsDead) return;

            var readySkill = _skillSlots.GetReadySkill();
            if (readySkill == null) return;

            float distance = Vector2.Distance(
                _player.transform.position,
                _currentTarget.transform.position);

            if (distance > readySkill.Data.Range) return;

            readySkill.Execute(_player.Stats, _currentTarget);
        }

        public SkillSlotManager SkillSlots => _skillSlots;

        private void EquipStartingSkills()
        {
            if (_startingSkills == null) return;

            for (int i = 0; i < _startingSkills.Length && i < _skillSlots.SlotCount; i++)
            {
                if (_startingSkills[i] != null)
                {
                    _skillSlots.EquipSkill(i, _startingSkills[i]);
                }
            }
        }

        private void HandleStateChange(GameState state)
        {
            _isActive = state == GameState.Playing;
        }
    }
}