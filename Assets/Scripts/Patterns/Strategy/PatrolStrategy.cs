using UnityEngine;
using RedRunner.Interfaces;

namespace RedRunner.Patterns.Strategy
{
    public class PatrolStrategy : IMovementStrategy
    {
        private Vector3 startPoint;
        private Vector3 endPoint;
        private Vector3 currentTarget;
        private bool movingToEnd = true;
        private Transform ownerTransform;
        private ref bool ownerFacingRight;

        public PatrolStrategy(Transform owner, float patrolDistance, ref bool facingRightRef)
        {
            ownerTransform = owner;
            startPoint = owner.position;
            endPoint = startPoint + Vector3.right * patrolDistance * Mathf.Sign(owner.localScale.x);
            currentTarget = endPoint;
            ownerFacingRight = facingRightRef;
        }

        public void Execute(Transform transform, float speed)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
            {
                TurnAround();
            }
        }

        public void TurnAround()
        {
            movingToEnd = !movingToEnd;
            currentTarget = movingToEnd ? endPoint : startPoint;
            ownerFacingRight = !ownerFacingRight;
        }
    }
}