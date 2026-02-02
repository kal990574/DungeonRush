using DungeonRush.Core;

namespace DungeonRush.UI.HUD
{
    public class HPBarPresenter : PresenterBase<HPBarModel, IHPBarView>
    {
        public HPBarPresenter(HPBarModel model, IHPBarView view) : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnPlayerHPChanged += HandlePlayerHPChanged;
            GameEventBus.OnPlayerDamaged += HandlePlayerDamaged;
            GameEventBus.OnPlayerHealed += HandlePlayerHealed;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnPlayerHPChanged -= HandlePlayerHPChanged;
            GameEventBus.OnPlayerDamaged -= HandlePlayerDamaged;
            GameEventBus.OnPlayerHealed -= HandlePlayerHealed;
        }

        protected override void HandleModelChanged()
        {
            View.SetFillAmount(Model.Ratio);
            View.SetText(Model.DisplayText);
        }

        private void HandlePlayerHPChanged(float currentHP, float maxHP)
        {
            Model.SetHP(currentHP, maxHP);
        }

        private void HandlePlayerDamaged(float damage)
        {
            View.PlayDamageAnimation();
        }

        private void HandlePlayerHealed(float amount)
        {
            View.PlayHealAnimation();
        }
    }
}
