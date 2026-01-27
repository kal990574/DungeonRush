
public interface IHUDView : IView
{
    void UpdateHPBar(float fillRatio);
    void UpdateHPText(float current, float max);
    void UpdateXPBar(float fillRatio);
    void UpdateXPText(int current, int required);
    void UpdateGold(int amount);
    void UpdateStage(int chapter, int wave, bool isBoss);
}
