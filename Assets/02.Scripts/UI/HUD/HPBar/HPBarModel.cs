namespace DungeonRush.UI.HUD
{
    public class HPBarModel : ViewModelBase
    {
        private float _currentHP;
        private float _maxHP;

        public float CurrentHP => _currentHP;
        public float MaxHP => _maxHP;

        public float Ratio => _maxHP > 0f ? _currentHP / _maxHP : 0f;

        public string DisplayText => $"{(int)_currentHP}/{(int)_maxHP}";

        public void SetHP(float currentHP, float maxHP)
        {
            _currentHP = currentHP;
            _maxHP = maxHP;
            NotifyChanged();
        }
    }
}
