using DungeonRush.Core;

namespace DungeonRush.UI.HUD
{
    public class XPBarPresenter : PresenterBase<XPBarModel, IXPBarView>
    {
        public XPBarPresenter(XPBarModel model, IXPBarView view) : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnPlayerXPChanged += HandlePlayerXPChanged;
            GameEventBus.OnPlayerLevelUp += HandlePlayerLevelUp;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnPlayerXPChanged -= HandlePlayerXPChanged;
            GameEventBus.OnPlayerLevelUp -= HandlePlayerLevelUp;
        }

        protected override void HandleModelChanged()
        {
            View.SetFillAmount(Model.Ratio);
            View.SetLevelText(Model.LevelText);
        }

        private void HandlePlayerXPChanged(float currentXP, float requiredXP)
        {
            Model.SetXP(currentXP, requiredXP);
        }

        private void HandlePlayerLevelUp(int newLevel)
        {
            Model.SetLevel(newLevel);
            View.PlayLevelUpAnimation();
        }
    }
}
