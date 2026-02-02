namespace DungeonRush.UI.HUD
{
    public interface IGoldDisplayView : IView
    {
        void SetGoldText(string text);
        void PlayGainAnimation();
    }
}
