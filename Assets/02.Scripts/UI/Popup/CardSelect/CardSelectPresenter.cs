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
            GameEventBus.OnCardRerolled += HandleCardRerolled;
            View.OnCardClicked += HandleCardClicked;
            View.OnRerollClicked += HandleRerollClicked;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnCardChoicesReady -= HandleCardChoicesReady;
            GameEventBus.OnCardRerolled -= HandleCardRerolled;
            View.OnCardClicked -= HandleCardClicked;
            View.OnRerollClicked -= HandleRerollClicked;
        }

        protected override void HandleModelChanged()
        {
            View.ShowCards(Model.Cards);
            UpdateRerollButtons();
        }

        private void HandleCardChoicesReady(CardData[] cards)
        {
            Model.SetCards(cards);
            View.Show();
        }

        private void HandleCardRerolled(int cardIndex, CardData newCard)
        {
            Model.ReplaceCard(cardIndex, newCard);
            View.ShowCard(cardIndex, newCard);
            UpdateRerollButtons();
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

        private void HandleRerollClicked(int cardIndex)
        {
            if (!Model.CanRerollCard(cardIndex))
            {
                return;
            }

            GameEventBus.PublishCardRerollRequested(cardIndex);
        }

        private void UpdateRerollButtons()
        {
            for (int i = 0; i < Model.Cards.Length; i++)
            {
                View.SetRerollText(i, Model.GetRerollText(i));
                View.SetRerollInteractable(i, Model.CanRerollCard(i));
            }
        }
    }
}
