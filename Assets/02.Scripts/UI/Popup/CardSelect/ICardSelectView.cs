using System;
using DungeonRush.Data;

namespace DungeonRush.UI.Popup
{
    public interface ICardSelectView : IView
    {
        event Action<int> OnCardClicked;
        event Action OnRerollClicked;

        void ShowCards(CardData[] cards);
        void SetRerollButtonText(string text);
        void SetRerollButtonInteractable(bool interactable);
    }
}
