namespace DungeonRush.UI.HUD
{
    public interface IHPBarView : IView
    {
        void SetFillAmount(float ratio);
        void SetText(string text);
        void PlayDamageAnimation();
        void PlayHealAnimation();
    }
}
