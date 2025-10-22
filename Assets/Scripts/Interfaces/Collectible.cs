using UnityEngine; 

namespace DinoRunner.Interfaces
{
    public interface Collectible
    {
        void Collect(GameObject collector);

        int GetValue(); // e.g., for coins or score items
        string GetIdentifier(); // e.g., "Coin", "HealthPack"
    }
}