using UnityEngine;

namespace DinoRunner.Interfaces
{
    public interface Movable
    {
        void Move(Vector2 direction); // Apply movement based on direction
        void ApplyForce(Vector2 force, ForceMode2D mode); // For physics-based movement
        Vector2 GetVelocity();
        void SetVelocity(Vector2 velocity);
        void Stop(); // Halt movement
    }
}