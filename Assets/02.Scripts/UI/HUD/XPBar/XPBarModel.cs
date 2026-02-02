namespace DungeonRush.UI.HUD
{
    public class XPBarModel : ViewModelBase
    {
        private float _currentXP;
        private float _requiredXP;
        private int _level;

        public float CurrentXP => _currentXP;
        public float RequiredXP => _requiredXP;
        public int Level => _level;

        public float Ratio => _requiredXP > 0f ? _currentXP / _requiredXP : 0f;

        public string LevelText => $"Lv.{_level}";

        public void SetXP(float currentXP, float requiredXP)
        {
            _currentXP = currentXP;
            _requiredXP = requiredXP;
            NotifyChanged();
        }

        public void SetLevel(int level)
        {
            _level = level;
            NotifyChanged();
        }
    }
}
