using System;
using System.Collections.Generic;
using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Runtime
{
    // 런 중 가변 상태 관리. 장착, 버프, 소스별 스탯 합산.
    public class RunState
    {
        private readonly GameData _gameData;
        private readonly StatSheet _statSheet = new();
        private readonly Dictionary<int, EquippedWeapon> _weapons = new();
        private readonly Dictionary<int, WeaponResolvedStats> _weaponStats = new();
        private readonly Dictionary<int, EquippedRune> _runes = new();
        private bool _isRunning;

        public event Action<string> OnStatChanged;
        public event Action<int, string> OnWeaponEquipped;
        public event Action<int> OnWeaponUnequipped;
        public event Action<int, int> OnWeaponLevelUp;

        public RunState(GameData gameData)
        {
            _gameData = gameData ?? throw new ArgumentNullException(nameof(gameData));
        }

        public bool IsRunning => _isRunning;

        public void StartRun(string characterId)
        {
            if (_isRunning)
            {
                return;
            }

            var character = _gameData.GetCharacter(characterId);
            if (character == null)
            {
                throw new ArgumentException($"Character not found: {characterId}");
            }

            _statSheet.Reset();
            _statSheet.SetBase(character.Stats);
            _weapons.Clear();
            _weaponStats.Clear();
            _runes.Clear();
            _isRunning = true;
        }

        public void EndRun()
        {
            _statSheet.Reset();
            _weapons.Clear();
            _weaponStats.Clear();
            _runes.Clear();
            _isRunning = false;
        }

        public float GetStat(string key)
        {
            return _statSheet.GetFinal(key);
        }

        public StatBreakdown GetBreakdown(string key)
        {
            return _statSheet.GetBreakdown(key);
        }

        public void AddModifier(StatModifier modifier)
        {
            _statSheet.AddModifier(modifier);
            MarkAllWeaponsDirty();
            OnStatChanged?.Invoke(modifier.StatKey);
        }

        public void RemoveModifiersBySource(string sourceKey)
        {
            _statSheet.RemoveModifiersBySource(sourceKey);
            MarkAllWeaponsDirty();
        }

        public void EquipWeapon(int slotIndex, string skillId)
        {
            var skill = _gameData.GetSkill(skillId);
            if (skill == null || !skill.IsAttack)
            {
                return;
            }

            var weapon = new EquippedWeapon(slotIndex, skillId);
            _weapons[slotIndex] = weapon;
            _weaponStats[slotIndex] = new WeaponResolvedStats(_gameData, weapon, _statSheet);

            OnWeaponEquipped?.Invoke(slotIndex, skillId);
        }

        public void UnequipWeapon(int slotIndex)
        {
            if (_weapons.Remove(slotIndex))
            {
                _weaponStats.Remove(slotIndex);
                OnWeaponUnequipped?.Invoke(slotIndex);
            }
        }

        public void LevelUpWeapon(int slotIndex)
        {
            if (!_weapons.TryGetValue(slotIndex, out var weapon))
            {
                return;
            }

            var spec = _gameData.GetSkill(weapon.SkillId);
            if (spec == null || weapon.CurrentLevel >= spec.MaxLevel)
            {
                return;
            }

            weapon.CurrentLevel++;

            if (_weaponStats.TryGetValue(slotIndex, out var resolved))
            {
                resolved.MarkDirty();
            }

            OnWeaponLevelUp?.Invoke(slotIndex, weapon.CurrentLevel);
        }

        public SkillAttackData GetWeaponStats(int slotIndex)
        {
            if (_weaponStats.TryGetValue(slotIndex, out var resolved))
            {
                return resolved.Resolve();
            }

            return null;
        }

        public EquippedWeapon GetWeapon(int slotIndex)
        {
            return _weapons.TryGetValue(slotIndex, out var weapon) ? weapon : null;
        }

        public void EquipRune(int slotIndex, string runeId)
        {
            _runes[slotIndex] = new EquippedRune(slotIndex, runeId);
        }

        public void UnequipRune(int slotIndex)
        {
            _runes.Remove(slotIndex);
        }

        public EquippedRune GetRune(int slotIndex)
        {
            return _runes.TryGetValue(slotIndex, out var rune) ? rune : null;
        }

        private void MarkAllWeaponsDirty()
        {
            foreach (var ws in _weaponStats.Values)
            {
                ws.MarkDirty();
            }
        }
    }
}
