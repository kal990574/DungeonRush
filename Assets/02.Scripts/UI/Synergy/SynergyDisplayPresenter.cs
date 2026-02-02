using DungeonRush.Core;
using DungeonRush.Data;

namespace DungeonRush.UI.Synergy
{
    public class SynergyDisplayPresenter : PresenterBase<SynergyDisplayModel, ISynergyDisplayView>
    {
        public SynergyDisplayPresenter(SynergyDisplayModel model, ISynergyDisplayView view)
            : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnSynergyActivated += HandleSynergyActivated;
            GameEventBus.OnSynergyDeactivated += HandleSynergyDeactivated;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnSynergyActivated -= HandleSynergyActivated;
            GameEventBus.OnSynergyDeactivated -= HandleSynergyDeactivated;
        }

        protected override void HandleModelChanged()
        {
            View.SetSynergies(Model.ActiveSynergies);
        }

        private void HandleSynergyActivated(SynergyData synergyData, SynergyTier tier)
        {
            Model.AddSynergy(synergyData, tier);
            View.PlayActivateAnimation();
        }

        private void HandleSynergyDeactivated(SynergyData synergyData)
        {
            Model.RemoveSynergy(synergyData);
            View.PlayDeactivateAnimation();
        }
    }
}
