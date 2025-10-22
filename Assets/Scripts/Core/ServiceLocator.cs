// --- Assets/Scripts/Core/ServiceLocator.cs ---
using UnityEngine;
using System;
using System.Collections.Generic;
using DinoRunner.Patterns.Singleton;

namespace DinoRunner.Core
{
    public class ServiceLocator : SingletonMonoBehaviour<ServiceLocator>
    {
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void RegisterService<T>(T service) where T : class
        {
            Type type = typeof(T);
            if (!services.ContainsKey(type))
            {
                services.Add(type, service);
            }
            else
            {
                services[type] = service;
            }
        }

        public T GetService<T>() where T : class
        {
            Type type = typeof(T);
            if (services.TryGetValue(type, out object serviceInstance))
            {
                return (T)serviceInstance;
            }
            Debug.LogError($"[ServiceLocator] Service not found: {type.Name}. Was it registered in GameInitializer?");
            return null;
        }

        public bool HasService<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        public void UnregisterService<T>() where T : class
        {
            Type type = typeof(T);
            services.Remove(type);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            services.Clear();
        }
    }
}