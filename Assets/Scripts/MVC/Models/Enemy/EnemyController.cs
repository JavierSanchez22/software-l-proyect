using UnityEngine;
using RedRunner.Interfaces;
using RedRunner.MVC.Models.Enemy;
using RedRunner.MVC.Views.Enemy;
using RedRunner.Patterns.Observer;

namespace RedRunner.MVC.Controllers.Enemy
{
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected EnemyView enemyView;
        [SerializeField] protected Transform playerTarget;
        
        protected EnemyModel enemyModel;
        protected IMovementStrategy movementStrategy;
        protected bool isFacingRight = true;
        
        protected virtual void Awake()
        {
            if (enemyView == null) enemyView = GetComponent<EnemyView>();
            if (playerTarget == null) FindPlayer();
            
            enemyModel = new EnemyModel(enemyData);
            InitializeMovementStrategy();
        }
        
        protected virtual void Update()
        {
            if (enemyModel.IsAlive)
            {
                movementStrategy?.Execute(transform, enemyData.moveSpeed);
            }
        }
        
        protected virtual void InitializeMovementStrategy()
        {
        }
        
        protected virtual void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
        
        public void TakeDamage(float damage)
        {
            enemyModel.TakeDamage(damage);
            enemyView.PlayHitEffect();
        }
        
        public float GetCurrentHealth() => enemyModel.GetCurrentHealth();
        public float GetMaxHealth() => enemyModel.GetMaxHealth();
        public bool IsAlive() => enemyModel.IsAlive;
    }
}