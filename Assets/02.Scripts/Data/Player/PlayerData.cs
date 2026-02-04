using UnityEngine;

namespace _02.Scripts.Data.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "DungeonRush/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Survival")]
        public float MaxHp = 100f;

        [Header("Combat")]
        public float DamageMultiplier = 1f;
        public float CritChance = 0f;
        public float CritDamage = 1.5f;

        [Header("Growth")]
        public float XpMultiplier = 1f;
    }
}
