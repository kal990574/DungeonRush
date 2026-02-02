namespace DungeonRush.UI.HUD
{
    public interface IStageDisplayView : IView
    {
        void SetStageText(string text);
        void ShowBossIndicator(bool show);
    }
}
