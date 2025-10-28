using UnityEngine;

namespace RedRunner.MVC.Models.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "RedRunner/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string enemyName = "DefaultEnemy";
        public float maxHealth = 50f;
        public float moveSpeed = 2f;
        public float attackDamage = 10f;
        public int scoreValue = 100;
        public float patrolDistance = 5f;
        public float chaseRange = 8f;
    }
}