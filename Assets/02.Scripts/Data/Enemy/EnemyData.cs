using UnityEngine;

namespace _02.Scripts.Data.Enemy
{
    [CreateAssetMenu(fileName = "EN_New", menuName = "DungeonRush/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string EnemyName;
        public float MaxHp = 50f;
        public float AttackDamage = 5f;
        public float AttackRange = 1.2f;
        public float AttackSpeed = 1f;
        public float MoveSpeed = 2f;
        public int XpReward = 10;
    }
}