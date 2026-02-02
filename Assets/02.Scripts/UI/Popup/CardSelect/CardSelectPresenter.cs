using DungeonRush.Core;
using DungeonRush.Data;

namespace DungeonRush.UI.Popup
{
    public class CardSelectPresenter : PresenterBase<CardSelectModel, ICardSelectView>
    {
        public CardSelectPresenter(CardSelectModel model, ICardSelectView view)
            : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnCardChoicesReady += HandleCardChoicesReady;
            GameEventBus.OnRerollUsed += HandleRerollUsed;
            View.OnCardClicked += HandleCardClicked;
            View.OnRerollClicked += HandleRerollClicked;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnCardChoicesReady -= HandleCardChoicesReady;
            GameEventBus.OnRerollUsed -= HandleRerollUsed;
            View.OnCardClicked -= HandleCardClicked;
            View.OnRerollClicked -= HandleRerollClicked;
        }

        protected override void HandleModelChanged()
        {
            View.ShowCards(Model.Cards);
            View.SetRerollButtonText(Model.RerollText);
            View.SetRerollButtonInteractable(Model.CanReroll);
        }

        private void HandleCardChoicesReady(CardData[] cards)
        {
            Model.SetCards(cards);
            View.Show();
        }

        private void HandleRerollUsed(int rerollsRemaining, int cost)
        {
            Model.SetRerollInfo(rerollsRemaining, cost);
        }

        private void HandleCardClicked(int cardIndex)
        {
            if (Model.Cards == null || cardIndex >= Model.Cards.Length)
            {
                return;
            }

            GameEventBus.PublishCardSelected(Model.Cards[cardIndex]);
            View.Hide();
        }

        private void HandleRerollClicked()
        {
            if (!Model.CanReroll)
            {
                return;
            }

            // 리롤 요청은 외부 시스템에서 처리.
            // Presenter는 결과(OnRerollUsed, OnCardChoicesReady)만 수신.
        }
    }
}
