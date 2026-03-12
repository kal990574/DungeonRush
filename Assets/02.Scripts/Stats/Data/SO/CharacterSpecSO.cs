using System;
using UnityEngine;

namespace DungeonRush.Stats.Data.SO
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "DungeonRush/Stats/Character Spec")]
    public class CharacterSpecSO : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _id;
        [SerializeField] private string _characterName;
        [SerializeField] private int _grade;
        [SerializeField] private string _modelPrefab;

        [Header("Starting Skills")]
        [SerializeField] private string _startActiveSkillId;
        [SerializeField] private string _startPassiveSkillId;

        [Header("Tags")]
        [SerializeField] private string _weaponTypeTag;
        [SerializeField] private string _characterTraitId;

        [Header("Base Stats")]
        [SerializeField] private float _atk;
        [SerializeField] private float _cdr;
        [SerializeField] private float _critRate;
        [SerializeField] private float _critDamage;
        [SerializeField] private float _hp;
        [SerializeField] private float _hpRegen;
        [SerializeField] private float _armor;
        [SerializeField] private float _evasion;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _pickupRange;
        [SerializeField] private float _expGain;
        [SerializeField] private float _goldGain;
        [SerializeField] private float _luck;
        [SerializeField] private float _lifesteal;
        [SerializeField] private float _thorns;

        public string Id => _id;

        public CharacterSpec ToCharacterSpec()
        {
            var stats = new BaseStats();
            stats.ATK = _atk;
            stats.CDR = _cdr;
            stats.CritRate = _critRate;
            stats.CritDamage = _critDamage;
            stats.HP = _hp;
            stats.HPRegen = _hpRegen;
            stats.Armor = _armor;
            stats.Evasion = _evasion;
            stats.MoveSpeed = _moveSpeed;
            stats.PickupRange = _pickupRange;
            stats.ExpGain = _expGain;
            stats.GoldGain = _goldGain;
            stats.Luck = _luck;
            stats.Lifesteal = _lifesteal;
            stats.Thorns = _thorns;

            return new CharacterSpec(
                _id, _characterName, _grade, _modelPrefab, stats,
                _startActiveSkillId, _startPassiveSkillId,
                _weaponTypeTag, _characterTraitId);
        }

#if UNITY_EDITOR
        public void SetFromCharacterSpec(CharacterSpec spec)
        {
            _id = spec.Id;
            _characterName = spec.Name;
            _grade = spec.Grade;
            _modelPrefab = spec.ModelPrefab;
            _startActiveSkillId = spec.StartActiveSkillId;
            _startPassiveSkillId = spec.StartPassiveSkillId;
            _weaponTypeTag = spec.WeaponTypeTag;
            _characterTraitId = spec.CharacterTraitId;

            _atk = spec.Stats.ATK;
            _cdr = spec.Stats.CDR;
            _critRate = spec.Stats.CritRate;
            _critDamage = spec.Stats.CritDamage;
            _hp = spec.Stats.HP;
            _hpRegen = spec.Stats.HPRegen;
            _armor = spec.Stats.Armor;
            _evasion = spec.Stats.Evasion;
            _moveSpeed = spec.Stats.MoveSpeed;
            _pickupRange = spec.Stats.PickupRange;
            _expGain = spec.Stats.ExpGain;
            _goldGain = spec.Stats.GoldGain;
            _luck = spec.Stats.Luck;
            _lifesteal = spec.Stats.Lifesteal;
            _thorns = spec.Stats.Thorns;
        }
#endif
    }
}
