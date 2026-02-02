using System.Collections.Generic;

namespace DungeonRush.UI.Synergy
{
    public interface ISynergyDisplayView : IView
    {
        void SetSynergies(IReadOnlyList<SynergyEntry> synergies);
        void PlayActivateAnimation();
        void PlayDeactivateAnimation();
    }
}
