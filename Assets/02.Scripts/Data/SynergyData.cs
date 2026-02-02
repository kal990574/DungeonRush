using UnityEngine;
using DungeonRush.Core;

namespace DungeonRush.Data
{
    [CreateAssetMenu(fileName = "SynergyData", menuName = "DungeonRush/Synergy Data")]
    public class SynergyData : ScriptableObject
    {
        [Header("Basic Info")]
        public string synergyName;
        public SynergyType synergyType;
        public Sprite icon;

        [Header("Tier Thresholds")]
        public int bronzeCount = 2;
        public int silverCount = 4;
        public int goldCount = 6;

        [Header("Description")]
        [TextArea(2, 4)]
        public string description;
    }
}
