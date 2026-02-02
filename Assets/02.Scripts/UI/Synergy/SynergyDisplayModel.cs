using System.Collections.Generic;
using DungeonRush.Core;
using DungeonRush.Data;

namespace DungeonRush.UI.Synergy
{
    public class SynergyDisplayModel : ViewModelBase
    {
        private readonly List<SynergyEntry> _activeSynergies = new List<SynergyEntry>();

        public IReadOnlyList<SynergyEntry> ActiveSynergies => _activeSynergies;

        public void AddSynergy(SynergyData synergyData, SynergyTier tier)
        {
            int existingIndex = _activeSynergies.FindIndex(
                entry => entry.SynergyData == synergyData);

            if (existingIndex >= 0)
            {
                _activeSynergies[existingIndex] = new SynergyEntry(synergyData, tier);
            }
            else
            {
                _activeSynergies.Add(new SynergyEntry(synergyData, tier));
            }

            NotifyChanged();
        }

        public void RemoveSynergy(SynergyData synergyData)
        {
            _activeSynergies.RemoveAll(entry => entry.SynergyData == synergyData);
            NotifyChanged();
        }
    }

    public readonly struct SynergyEntry
    {
        public SynergyData SynergyData { get; }
        public SynergyTier Tier { get; }

        public SynergyEntry(SynergyData synergyData, SynergyTier tier)
        {
            SynergyData = synergyData;
            Tier = tier;
        }
    }
}
