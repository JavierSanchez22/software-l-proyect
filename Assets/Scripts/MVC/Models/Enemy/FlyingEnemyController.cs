using UnityEngine;
using RedRunner.Patterns.Strategy;

namespace RedRunner.MVC.Controllers.Enemy
{
    public class FlyingEnemyController : EnemyController
    {
        protected override void InitializeMovementStrategy()
        {
            if (playerTarget != null)
            {
                movementStrategy = new ChaseStrategy(playerTarget, enemyData.chaseRange, 1f);
            }
        }
        
        protected override void Update()
        {
            if (playerTarget == null) FindPlayer();
            
            if (movementStrategy == null && playerTarget != null)
            {
                movementStrategy = new ChaseStrategy(playerTarget, enemyData.chaseRange, 1f);
            }
            
            base.Update();
            
            UpdateFacingDirection();
        }
        
        private void UpdateFacingDirection()
        {
            if (playerTarget != null)
            {
                bool shouldFaceRight = playerTarget.position.x > transform.position.x;
                if (shouldFaceRight != isFacingRight)
                {
                    isFacingRight = shouldFaceRight;
                    enemyView.SetFacing(isFacingRight);
                }
            }
        }
    }
}