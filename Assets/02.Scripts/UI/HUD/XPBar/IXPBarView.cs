namespace DungeonRush.UI.HUD
{
    public interface IXPBarView : IView
    {
        void SetFillAmount(float ratio);
        void SetLevelText(string text);
        void PlayLevelUpAnimation();
    }
}
