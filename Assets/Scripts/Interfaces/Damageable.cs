namespace DinoRunner.Interfaces
{
    public interface Damageable
    {
        void TakeDamage(float damage);
        float GetCurrentHealth();
        float GetMaxHealth();
        bool IsAlive();
    }
}