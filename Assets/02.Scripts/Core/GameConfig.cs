using UnityEngine;

namespace _02.Scripts.Core
{
    public class GameConfig : ScriptableObject
    {
        [Header("Player Initial Stats")]
        public float PlayerMaxHp = 100f;
        public float PlayerAttackPower = 10f;
        public float PlayerAttackSpeed = 1f;
        
        [Header("Stage Settings")]
        public float TargetSearchInterval = 0.2f;
        public float DamageTextDuration = 0.5f;
        
        [Header("LevelUp Settings")]
        public int BaseXpRequired = 100;
        public float XpMultiplierPerLevel = 1.2f;
        public int CardChoiceCount = 3;
        
        [Header("Reroll Settings")] 
        public int BaseRerollCost = 40;
        public float RerollConstMultiplier = 1.2f;
        public int MaxRerollCount = 3;
    }
}