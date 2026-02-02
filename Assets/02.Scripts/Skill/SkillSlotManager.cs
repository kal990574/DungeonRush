using _02.Scripts.Data.Skill;
using UnityEngine;

namespace _02.Scripts.Skill
{
    public class SkillSlotManager
    {
        private const int MaxSlots = 3;
        private readonly Skill[] _slots = new Skill[MaxSlots];

        public int SlotCount => MaxSlots;

        public bool EquipSkill(int slotIndex, SkillData data)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlots) return false;

            _slots[slotIndex] = new Skill(data);
            return true;
        }

        public void UnequipSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlots) return;

            _slots[slotIndex] = null;
        }

        public Skill GetReadySkill()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (_slots[i] != null && _slots[i].IsReady)
                {
                    return _slots[i];
                }
            }

            return null;
        }

        public Skill GetSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= MaxSlots) return null;

            return _slots[slotIndex];
        }

        public void UpdateAllCooldowns(float deltaTime)
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                _slots[i]?.UpdateCooldown(deltaTime);
            }
        }
    }
}