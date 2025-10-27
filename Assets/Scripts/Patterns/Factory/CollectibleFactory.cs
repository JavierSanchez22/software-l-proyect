using UnityEngine;
using System.Collections.Generic;

using DinoRunner.Patterns.Singleton;

namespace DinoRunner.Patterns.Factory
{
    public class CollectibleFactory : 
        SingletonMonoBehaviour<CollectibleFactory>
    {
        [System.Serializable]
        public class CollectiblePrefabMapping
        {
            public string collectibleTypeIdentifier;
            public GameObject collectiblePrefab;
        }

        [Header("Collectible Prefabs")]
        [SerializeField] private List<CollectiblePrefabMapping> collectiblePrefabs;
        
        [SerializeField] private Transform collectibleContainer;

        protected override void Awake()
        {
            base.Awake();
            if (collectibleContainer == null)
            {
                collectibleContainer = new GameObject("---Collectibles---").transform;
            }
        }

        public GameObject CreateCollectible(string typeIdentifier, Vector3 position)
        {
            CollectiblePrefabMapping mapping = collectiblePrefabs.Find(m =>
            m.collectibleTypeIdentifier == typeIdentifier);

            if (mapping == null || mapping.collectiblePrefab == null)
            {
                Debug.LogError($"[CollectibleFactory] Prefab not found for type: {typeIdentifier} ");
                return null;
            }

            GameObject instance = Instantiate(mapping.collectiblePrefab,
            position, Quaternion.identity, collectibleContainer);

            instance.name = $"{typeIdentifier}_{Time.time}";
            return instance;
        }
    }
}