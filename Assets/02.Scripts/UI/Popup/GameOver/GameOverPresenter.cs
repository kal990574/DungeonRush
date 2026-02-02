using DungeonRush.Core;

namespace DungeonRush.UI.Popup
{
    public class GameOverPresenter : PresenterBase<GameOverModel, IGameOverView>
    {
        public GameOverPresenter(GameOverModel model, IGameOverView view)
            : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnGameOver += HandleGameOver;
            View.OnRetryClicked += HandleRetryClicked;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnGameOver -= HandleGameOver;
            View.OnRetryClicked -= HandleRetryClicked;
        }

        protected override void HandleModelChanged()
        {
            View.SetStageText(Model.StageText);
            View.SetKillsText(Model.Kills.ToString());
            View.SetTimeText(Model.TimeText);
            View.SetScoreText(Model.Score.ToString("N0"));
        }

        private void HandleGameOver(int chapter, int wave, int kills, float time, int score)
        {
            Model.SetResult(chapter, wave, kills, time, score);
            View.Show();
        }

        private void HandleRetryClicked()
        {
            View.Hide();
            GameEventBus.PublishGameStateChanged(GameState.Loading);
        }
    }
}
