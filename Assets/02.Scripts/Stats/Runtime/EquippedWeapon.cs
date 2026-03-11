namespace DungeonRush.Stats.Runtime
{
    public class EquippedWeapon
    {
        public int SlotIndex;
        public string SkillId;
        public int CurrentLevel;

        public EquippedWeapon(int slotIndex, string skillId, int currentLevel = 1)
        {
            SlotIndex = slotIndex;
            SkillId = skillId;
            CurrentLevel = currentLevel;
        }
    }
}
