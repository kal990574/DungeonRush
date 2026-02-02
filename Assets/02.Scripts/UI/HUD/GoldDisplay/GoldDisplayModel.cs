namespace DungeonRush.UI.HUD
{
    public class GoldDisplayModel : ViewModelBase
    {
        private int _currentGold;

        public int CurrentGold => _currentGold;

        public string DisplayText => _currentGold.ToString("N0");

        public void SetGold(int gold)
        {
            _currentGold = gold;
            NotifyChanged();
        }
    }
}
