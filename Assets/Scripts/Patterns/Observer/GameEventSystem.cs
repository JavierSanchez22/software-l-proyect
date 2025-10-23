using System;
using System.Collections.Generic;
using RedRunner.Patterns.Singleton;
using RedRunner.MVC.Models.Player; // For HealthData struct
namespace RedRunner.Patterns.Observer
{

    public class GameEventSystem : SingletonMonoBehaviour<GameEventSystem>
    {
        private readonly Dictionary<GameEvents, Action<object>> eventDictionary = new Dictionary<GameEvents, Action<object>>();
        public void Subscribe(GameEvents eventType, Action<object> listener)
        {
            if (listener == null) return;
            if (!eventDictionary.ContainsKey(eventType) eventDictionary.Add(eventType, null);
            eventDictionary[eventType] += listener;
        }
        public void Unsubscribe(GameEvents eventType, Action<object> listener)
        {
            if (listener == null || !eventDictionary.ContainsKey(eventType))
                return;
            eventDictionary[eventType] -= listener;
            if (eventDictionary[eventType] == null)
                eventDictionary.Remove(eventType);
        }
        public void Publish(GameEvents eventType, object eventData = null)
        {
            if (eventDictionary.TryGetValue(eventType, out Action<object>
            thisEvent))
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
