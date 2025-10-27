using UnityEngine;
using DinoRunner.Interfaces; 
using DinoRunner.Core; 
using DinoRunner.Patterns.Observer; 

namespace DinoRunner.MVC.Models.Collectibles
{
    public class Coin : CollectibleBase
    {
        protected override void Awake()
        {
            base.Awake();
            identifier = "Coin";
            collectSoundName = "coin_collect"; // Use specific sound name from AudioService
            value = 1; // Or read from GameConfig/LevelData
        }

        protected override void PerformCollectAction(GameObject collector)
        {
            // Option 1: Directly modify PlayerModel if accessible (requires reference or event)
            // PlayerController player = collector.GetComponent<PlayerController>();
            // player?.GetModel().AddCoins(value);

            // Option 2: Publish an event that ScoreService/PlayerModel listens to
            GameEventSystem.Instance.Publish(GameEvents.COIN_COLLECTED, value);
        }
    }
}