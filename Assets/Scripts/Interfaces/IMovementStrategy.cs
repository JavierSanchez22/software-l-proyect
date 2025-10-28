using UnityEngine;

namespace RedRunner.Interfaces
{
    public interface IMovementStrategy
    {
        void Execute(Transform transform, float speed);
    }
}