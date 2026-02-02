using System;
using DungeonRush.Data;

namespace DungeonRush.UI.Popup
{
    public interface ICardSelectView : IView
    {
        event Action<int> OnCardClicked;
        event Action<int> OnRerollClicked;

        void ShowCards(CardData[] cards);
        void ShowCard(int index, CardData card);
        void SetRerollText(int index, string text);
        void SetRerollInteractable(int index, bool interactable);
    }
}
