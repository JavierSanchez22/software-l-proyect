using UnityEngine;
using System.Collections.Generic;
using RedRunner.Patterns.Singleton;

namespace RedRunner.Patterns.Factory
{
    public class EnemyFactory : SingletonMonoBehaviour<EnemyFactory>
    {
        [System.Serializable]
        public class EnemyPrefabMapping
        {
            public string enemyTypeIdentifier;
            public GameObject enemyPrefab;
        }

        [SerializeField] private List<EnemyPrefabMapping> enemyPrefabs;
        [SerializeField] private Transform enemyContainer;

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
            EnemyPrefabMapping mapping = enemyPrefabs.Find(m => m.enemyTypeIdentifier == typeIdentifier);
            if (mapping == null || mapping.enemyPrefab == null)
            {
                Debug.LogError($"[EnemyFactory] Prefab not found for type: {typeIdentifier}");
                return null;
            }

            GameObject enemyInstance = Instantiate(mapping.enemyPrefab, position, Quaternion.identity, enemyContainer);
            enemyInstance.name = $"{typeIdentifier}.{Time.time}";
            return enemyInstance;
        }
    }
}