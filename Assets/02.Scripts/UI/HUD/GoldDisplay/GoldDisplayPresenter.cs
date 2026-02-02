using DungeonRush.Core;

namespace DungeonRush.UI.HUD
{
    public class GoldDisplayPresenter : PresenterBase<GoldDisplayModel, IGoldDisplayView>
    {
        public GoldDisplayPresenter(GoldDisplayModel model, IGoldDisplayView view)
            : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnGoldChanged += HandleGoldChanged;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnGoldChanged -= HandleGoldChanged;
        }

        protected override void HandleModelChanged()
        {
            View.SetGoldText(Model.DisplayText);
        }

        private void HandleGoldChanged(int currentGold, int delta)
        {
            Model.SetGold(currentGold);

            if (delta > 0)
            {
                View.PlayGainAnimation();
            }
        }
    }
}
