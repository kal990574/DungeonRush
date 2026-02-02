using DungeonRush.Data;

namespace DungeonRush.UI.HUD
{
    public class SkillButtonModel : ViewModelBase
    {
        private SkillData _skillData;
        private float _cooldownRemaining;
        private float _cooldownTotal;
        private bool _isLocked;
        private int _slotIndex;

        public SkillData SkillData => _skillData;
        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownTotal => _cooldownTotal;
        public bool IsLocked => _isLocked;
        public int SlotIndex => _slotIndex;

        public bool IsOnCooldown => _cooldownRemaining > 0f;

        public float CooldownRatio => _cooldownTotal > 0f
            ? _cooldownRemaining / _cooldownTotal
            : 0f;

        public SkillButtonModel(int slotIndex)
        {
            _slotIndex = slotIndex;
            _isLocked = true;
        }

        public void EquipSkill(SkillData skillData)
        {
            _skillData = skillData;
            _isLocked = false;
            _cooldownRemaining = 0f;
            _cooldownTotal = 0f;
            NotifyChanged();
        }

        public void UnequipSkill()
        {
            _skillData = null;
            _isLocked = true;
            _cooldownRemaining = 0f;
            _cooldownTotal = 0f;
            NotifyChanged();
        }

        public void UpdateCooldown(float remaining, float total)
        {
            _cooldownRemaining = remaining;
            _cooldownTotal = total;
            NotifyChanged();
        }
    }
}
