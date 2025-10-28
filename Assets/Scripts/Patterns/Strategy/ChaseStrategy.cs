using UnityEngine;
using RedRunner.Interfaces;

namespace RedRunner.Patterns.Strategy
{
    public class ChaseStrategy : IMovementStrategy
    {
        private Transform target;
        private float detectionRange;
        private float stoppingDistance;

        public ChaseStrategy(Transform targetToChase, float range, float stopDistance = 0.5f)
        {
            target = targetToChase;
            detectionRange = range;
            stoppingDistance = stopDistance;
        }

        public void Execute(Transform transform, float speed)
        {
            if (target == null) return;

            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= detectionRange && distance > stoppingDistance)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }

        public void UpdateTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}