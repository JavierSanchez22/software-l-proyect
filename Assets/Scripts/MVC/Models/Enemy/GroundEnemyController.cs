using UnityEngine;
using RedRunner.Patterns.Strategy;

namespace RedRunner.MVC.Controllers.Enemy
{
    public class GroundEnemyController : EnemyController
    {
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckDistance = 0.5f;
        
        protected override void InitializeMovementStrategy()
        {
            movementStrategy = new PatrolStrategy(transform, enemyData.patrolDistance, ref isFacingRight);
        }
        
        protected override void Update()
        {
            base.Update();
            CheckForEdges();
        }
        
        private void CheckForEdges()
        {
            if (groundCheck == null) return;
            
            Vector2 checkDirection = isFacingRight ? Vector2.right : Vector2.left;
            RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, checkDirection, groundCheckDistance, groundLayer);
            
            if (hit.collider == null)
            {
                TurnAround();
            }
        }
        
        private void TurnAround()
        {
            if (movementStrategy is PatrolStrategy patrolStrategy)
            {
                patrolStrategy.TurnAround();
                enemyView.SetFacing(isFacingRight);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Vector2 checkDirection = isFacingRight ? Vector2.right : Vector2.left;
                Gizmos.DrawLine(groundCheck.position, groundCheck.position + (Vector3)(checkDirection * groundCheckDistance));
            }
        }
    }
}