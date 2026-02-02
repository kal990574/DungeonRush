using DungeonRush.Data;

namespace DungeonRush.UI.Popup
{
    public class CardSelectModel : ViewModelBase
    {
        private const int CardCount = 3;
        private const int MaxRerollPerCard = 3;

        private CardData[] _cards;
        private readonly int[] _rerollCounts = new int[CardCount];

        public CardData[] Cards => _cards;

        public bool CanRerollCard(int index)
        {
            return _rerollCounts[index] < MaxRerollPerCard;
        }

        public int GetRerollCount(int index)
        {
            return _rerollCounts[index];
        }

        public string GetRerollText(int index)
        {
            if (!CanRerollCard(index))
            {
                return "MAX";
            }

            int remaining = MaxRerollPerCard - _rerollCounts[index];
            return $"({remaining})";
        }

        public void SetCards(CardData[] cards)
        {
            _cards = cards;

            for (int i = 0; i < CardCount; i++)
            {
                _rerollCounts[i] = 0;
            }

            NotifyChanged();
        }

        public void ReplaceCard(int index, CardData newCard)
        {
            _cards[index] = newCard;
            _rerollCounts[index]++;
            NotifyChanged();
        }
    }
}
