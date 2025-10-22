using UnityEngine;

namespace DinoRunner.Interfaces
{
    public interface MovementStrategy
    {
        void Execute(Transform transform, float speed);
    }
}