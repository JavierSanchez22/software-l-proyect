using UnityEngine;
using DinoRunner.MVC.Models.Enemy; // For EnemyData, EnemyController etc.
using System.Collections.Generic; // For lists if using pooling
namespace DinoRunner.Patterns.Factory
{
    public class EnemyFactory : Singleton.SingletonMonoBehaviour<EnemyFactory>
    {
        [System.Serializable]
        public class EnemyPrefabMapping
        {
            public string enemyTypeIdentifier; // e.g., "GroundPatrol", "FlyingChase"
            public GameObject enemyPrefab; // Assign prefab in Inspector
        }
        [Header("Enemy Prefabs")]
        [SerializeField] private List<EnemyPrefabMapping> enemyPrefabs;
        [SerializeField] private Transform enemyContainer; // Optional parent for spawned enemies
        // TODO: Implement Object Pooling here if desired
        protected override void Awake()
        {
            base.Awake();
            if (enemyContainer == null)
            {
                enemyContainer = new GameObject("---Enemies---").transform;
            }
        }

        public GameObject CreateEnemy(string typeIdentifier, Vector3 position)
        {
            EnemyPrefabMapping mapping = enemyPrefabs.Find(m =>
            m.enemyTypeIdentifier == typeIdentifier);
            if (mapping == null || mapping.enemyPrefab == null)
            {
                Debug.LogError($"[EnemyFactory] Prefab not found for type: {typeIdentifier}");
                return null;
            }
            GameObject enemyInstance = Instantiate(mapping.enemyPrefab,
                position, Quaternion.identity, enemyContainer);
            enemyInstance.name = $"{typeIdentifier}_{Time.time}"; // Unique name
            // Optionally initialize components here if needed, though
            // Awake/Start is preferred
            EnemyController controller = enemyInstance.GetComponent<EnemyController>();
            // controller?.Initialize(...);
            return enemyInstance;
        }
    }
}
