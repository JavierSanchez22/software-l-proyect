using UnityEngine;
using RedRunner.Interfaces;

namespace RedRunner.Patterns.Strategy
{
    public class IdleStrategy : IMovementStrategy
    {
        public void Execute(Transform transform, float speed)
        {
        }
    }
}