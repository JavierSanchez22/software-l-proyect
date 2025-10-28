using UnityEngine;
using RedRunner.Interfaces;
using RedRunner.Patterns.Observer;

namespace RedRunner.MVC.Models.Enemy
{
    public class EnemyModel : IDamageable
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public int ScoreValue { get; private set; }
        public float AttackDamage { get; private set; }
        public bool IsAlive { get; private set; }

        private EnemyData enemyData;

        public EnemyModel(EnemyData data)
        {
            if (data == null) return;

            enemyData = data;
            MaxHealth = enemyData.maxHealth;
            CurrentHealth = MaxHealth;
            ScoreValue = enemyData.scoreValue;
            AttackDamage = enemyData.attackDamage;
            IsAlive = true;
        }

        public void TakeDamage(float damageAmount)
        {
            if (!IsAlive || damageAmount <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - damageAmount);
            GameEventSystem.Instance.Publish(GameEvents.ENEMY_HIT, this);

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (!IsAlive) return;
            IsAlive = false;
            GameEventSystem.Instance.Publish(GameEvents.ENEMY_KILLED, this);
        }

        public float GetCurrentHealth() => CurrentHealth;
        public float GetMaxHealth() => MaxHealth;

        public void Reset()
        {
            CurrentHealth = MaxHealth;
            IsAlive = true;
        }
    }
}