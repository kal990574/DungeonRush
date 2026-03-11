namespace DungeonRush.Stats.Runtime
{
    public class EquippedRune
    {
        public int SlotIndex;
        public string RuneId;
        public int CurrentLevel;

        public EquippedRune(int slotIndex, string runeId, int currentLevel = 1)
        {
            SlotIndex = slotIndex;
            RuneId = runeId;
            CurrentLevel = currentLevel;
        }
    }
}
