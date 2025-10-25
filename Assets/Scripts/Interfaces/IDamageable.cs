using UnityEngine;
namespace Assets.Scripts.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        float GetCurrentHealth();
        float GetMaxHealth();
        bool IsAlive();
    }
}