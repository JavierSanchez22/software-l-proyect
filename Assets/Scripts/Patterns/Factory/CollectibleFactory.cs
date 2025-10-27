using UnityEngine;
using System.Collections.Generic;

namespace DinoRunner.Patterns.Factory
{
    public class CollectibleFactory :
    Singleton.SingletonMonoBehaviour<CollectibleFactory>
    {
        [System.Serializable]
        public class CollectiblePrefabMapping
        {
            public string collectibleTypeIdentifier; // e.g., "Coin", "HealthPack"
            public GameObject collectiblePrefab;
        }
        [Header("Collectible Prefabs")]
        [SerializeField] private List<CollectiblePrefabMapping> collectiblePrefabs;
        [SerializeField] private Transform collectibleContainer;
        // TODO: Implement Object Pooling
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
                Debug.LogError($"[CollectibleFactory] Prefab not found for type: { typeIdentifier} ");
                return null;
            }
            // TODO: Replace Instantiate with pool logic
            GameObject instance = Instantiate(mapping.collectiblePrefab,
            position, Quaternion.identity, collectibleContainer);
            instance.name = $"{typeIdentifier}_{Time.time}";
            return instance;
        }
    }


public GameObject CreateCollectible(string typeIdentifier, Vector3 position)
        {
            CollectiblePrefabMapping mapping = collectiblePrefabs.Find(m =>
            m.collectibleTypeIdentifier == typeIdentifier);
            if (mapping == null || mapping.collectiblePrefab == null)
            {
                Debug.LogError($"[CollectibleFactory] Prefab not found for type: { typeIdentifier} ");
                return null;
            }

            // TODO: Replace Instantiate with pool logic
            GameObject instance = Instantiate(mapping.collectiblePrefab,
            position, Quaternion.identity, collectibleContainer);
            instance.name = $"{typeIdentifier}_{Time.time}";
            return instance;
        }
    }
}