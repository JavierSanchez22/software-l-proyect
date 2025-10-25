using System;
using System.Collections.Generic;
using Assets.Scripts.Patterns.Singleton;
using Assets.Scripts.MVC.Models.Player;

namespace Assets.Scripts.Patterns.Observer
{
    public class GameEventSystem : SingletonMonoBehaviour<GameEventSystem>
    {
        private readonly Dictionary<GameEvents, Action<object>> eventDictionary = new Dictionary<GameEvents, Action<object>>();

        public void Subscribe(GameEvents eventType, Action<object> listener)
        {
            if (listener == null) return;
            if (!eventDictionary.ContainsKey(eventType)) eventDictionary.Add(eventType, null);
            eventDictionary[eventType] += listener;
        }

        public void Unsubscribe(GameEvents eventType, Action<object> listener)
        {
            if (listener == null || !eventDictionary.ContainsKey(eventType)) return;
            eventDictionary[eventType] -= listener;
            if (eventDictionary[eventType] == null) eventDictionary.Remove(eventType);
        }

        public void Publish(GameEvents eventType, object eventData = null)
        {
            if (eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
            {
                thisEvent?.Invoke(eventData);
            }
        }

        protected override void OnDestroy()
        {
            eventDictionary.Clear();
            base.OnDestroy();
        }
    }

    // Enum containing all possible game events
    public enum GameEvents
    {
        // Player State
        PLAYER_DIED, // No data needed, signals final death
        PLAYER_RESPAWNED, // No data needed
        PLAYER_HIT, // Could pass PlayerModel or damage amount
        PLAYER_INVULNERABILITY_STATUS_CHANGED, // bool: isInvulnerable

        // Player Stats Update
        HEALTH_CHANGED, // PlayerModel.HealthData { Current, Max }
        SCORE_CHANGED,  // int: new score
        COINS_CHANGED,  // int: new coin count
        LIVES_CHANGED,  // int: new lives count

        // Game State Control
        GAME_STARTED, // Signals game loop should begin
        GAME_PAUSED,
        GAME_RESUMED,
        GAME_OVER,
        LEVEL_COMPLETE, // Or GAME_VICTORY
        GAME_RESTARTED, // Signals scene reload/reset

        // UI Control (Alternative to direct GameManager calls)
        SHOW_SCREEN, // UIScreenInfo enum value
        HIDE_SCREEN, // UIScreenInfo enum value

        // Collectibles
        COIN_COLLECTED,     // int: value collected (usually 1)
        CHEST_OPENED,       // No data needed, or maybe position
        POWERUP_COLLECTED,  // string or enum: type of powerup

        // Enemies
        ENEMY_KILLED, // EnemyModel or int: score value
        ENEMY_HIT,    // EnemyModel

        // Audio
        AUDIO_STATE_CHANGED, // bool: isEnabled (for UI icon)
    }
}