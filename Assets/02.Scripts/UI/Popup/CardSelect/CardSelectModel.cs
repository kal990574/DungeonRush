using DungeonRush.Data;

namespace DungeonRush.UI.Popup
{
    public class CardSelectModel : ViewModelBase
    {
        private CardData[] _cards;
        private int _rerollCost;
        private int _rerollsRemaining;

        public CardData[] Cards => _cards;
        public int RerollCost => _rerollCost;
        public int RerollsRemaining => _rerollsRemaining;

        public bool CanReroll => _rerollsRemaining > 0;

        public string RerollText => CanReroll
            ? $"Reroll ({_rerollCost}G) [{_rerollsRemaining}]"
            : "No Rerolls";

        public void SetCards(CardData[] cards)
        {
            _cards = cards;
            NotifyChanged();
        }

        public void SetRerollInfo(int rerollsRemaining, int cost)
        {
            _rerollsRemaining = rerollsRemaining;
            _rerollCost = cost;
            NotifyChanged();
        }
    }
}
