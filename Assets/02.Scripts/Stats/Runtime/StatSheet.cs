using System.Collections.Generic;
using DungeonRush.Stats.Config;
using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Runtime
{
    // 기본 스탯 + modifier 합산 → 최종 스탯 계산.
    // 공식: finalValue = (base + flatSum) * (1 + percentSum).
    public class StatSheet
    {
        private readonly Dictionary<string, float> _baseValues = new();
        private readonly List<StatModifier> _modifiers = new();
        private readonly Dictionary<string, float> _cachedFinals = new();
        private bool _isDirty = true;

        public void SetBase(BaseStats stats)
        {
            _baseValues.Clear();

            foreach (string key in BaseStats.AllKeys)
            {
                _baseValues[key] = stats.GetValue(key);
            }

            MarkDirty();
        }

        public void SetBase(string key, float value)
        {
            _baseValues[key] = value;
            MarkDirty();
        }

        public float GetBase(string key)
        {
            return _baseValues.TryGetValue(key, out float value) ? value : 0f;
        }

        public void AddModifier(StatModifier modifier)
        {
            _modifiers.Add(modifier);
            MarkDirty();
        }

        public void RemoveModifiersBySource(string sourceKey)
        {
            _modifiers.RemoveAll(m => m.SourceKey == sourceKey);
            MarkDirty();
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
            MarkDirty();
        }

        public float GetFinal(string key)
        {
            if (!_isDirty && _cachedFinals.TryGetValue(key, out float cached))
            {
                return cached;
            }

            if (_isDirty)
            {
                RecalculateAll();
            }

            return _cachedFinals.TryGetValue(key, out float val) ? val : GetBase(key);
        }

        public StatBreakdown GetBreakdown(string key)
        {
            float baseValue = GetBase(key);
            var entries = new List<StatBreakdownEntry>();
            float flatSum = 0f;
            float percentSum = 0f;

            foreach (var mod in _modifiers)
            {
                if (mod.StatKey != key)
                {
                    continue;
                }

                entries.Add(new StatBreakdownEntry(mod.SourceKey, mod.ModifyType, mod.Value));

                if (mod.ModifyType == ModifyType.Flat)
                {
                    flatSum += mod.Value;
                }
                else
                {
                    percentSum += mod.Value;
                }
            }

            float finalValue = (baseValue + flatSum) * (1f + percentSum);
            finalValue = StatConfig.ClampStat(key, finalValue);

            return new StatBreakdown(key, baseValue, finalValue, entries);
        }

        public void Reset()
        {
            _baseValues.Clear();
            _modifiers.Clear();
            _cachedFinals.Clear();
            _isDirty = true;
        }

        private void RecalculateAll()
        {
            _cachedFinals.Clear();

            var allKeys = new HashSet<string>(BaseStats.AllKeys);
            foreach (var mod in _modifiers)
            {
                allKeys.Add(mod.StatKey);
            }

            foreach (string key in allKeys)
            {
                float baseValue = GetBase(key);
                float flatSum = 0f;
                float percentSum = 0f;

                foreach (var mod in _modifiers)
                {
                    if (mod.StatKey != key)
                    {
                        continue;
                    }

                    if (mod.ModifyType == ModifyType.Flat)
                    {
                        flatSum += mod.Value;
                    }
                    else
                    {
                        percentSum += mod.Value;
                    }
                }

                float finalValue = (baseValue + flatSum) * (1f + percentSum);
                finalValue = StatConfig.ClampStat(key, finalValue);
                _cachedFinals[key] = finalValue;
            }

            _isDirty = false;
        }

        private void MarkDirty()
        {
            _isDirty = true;
        }
    }
}
