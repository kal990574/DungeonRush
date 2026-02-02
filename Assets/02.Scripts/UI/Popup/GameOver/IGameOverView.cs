using System;

namespace DungeonRush.UI.Popup
{
    public interface IGameOverView : IView
    {
        event Action OnRetryClicked;

        void SetStageText(string text);
        void SetKillsText(string text);
        void SetTimeText(string text);
        void SetScoreText(string text);
    }
}
