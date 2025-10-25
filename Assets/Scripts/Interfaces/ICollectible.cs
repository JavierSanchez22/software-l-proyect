using UnityEngine; 

namespace Assets.Scripts.Interfaces
{
    public interface ICollectible
    {
        void Collect(GameObject collector);

        int GetValue(); // e.g., for coins or score items
        string GetIdentifier(); // e.g., "Coin", "HealthPack"
    }
}