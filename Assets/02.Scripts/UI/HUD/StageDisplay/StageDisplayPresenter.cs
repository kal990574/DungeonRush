using DungeonRush.Core;

namespace DungeonRush.UI.HUD
{
    public class StageDisplayPresenter : PresenterBase<StageDisplayModel, IStageDisplayView>
    {
        public StageDisplayPresenter(StageDisplayModel model, IStageDisplayView view)
            : base(model, view) { }

        protected override void SubscribeEvents()
        {
            GameEventBus.OnWaveStart += HandleWaveStart;
            GameEventBus.OnBossWaveStart += HandleBossWaveStart;
        }

        protected override void UnsubscribeEvents()
        {
            GameEventBus.OnWaveStart -= HandleWaveStart;
            GameEventBus.OnBossWaveStart -= HandleBossWaveStart;
        }

        protected override void HandleModelChanged()
        {
            View.SetStageText(Model.StageText);
            View.ShowBossIndicator(Model.IsBossWave);
        }

        private void HandleWaveStart(int chapter, int wave)
        {
            Model.SetStage(chapter, wave, false);
        }

        private void HandleBossWaveStart(int chapter, int wave)
        {
            Model.SetStage(chapter, wave, true);
        }
    }
}
