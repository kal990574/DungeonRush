using UnityEngine;

namespace _02.Scripts.Data.Enemy
{
    [CreateAssetMenu(fileName = "EN_New", menuName = "DungeonRush/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName;
        public float maxHp = 50f;
        public float attackDamage = 5f;
        public float attackRange = 1.2f;
        public float attackSpeed = 1f;
        public float moveSpeed = 2f;
        public int xpReward = 10;
    }
}