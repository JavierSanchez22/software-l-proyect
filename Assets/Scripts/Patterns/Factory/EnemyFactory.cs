using UnityEngine;
using DinoRunner.MVC.Models.Enemy; 
using System.Collections.Generic;
namespace DinoRunner.Patterns.Factory
{
    public class EnemyFactory : Singleton.SingletonMonoBehaviour<EnemyFactory>
    {
        [System.Serializable]
        public class EnemyPrefabMapping
        {
            public string enemyTypeIdentifier; 
            public GameObject enemyPrefab;
        }
        [Header("Enemy Prefabs")]
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
            EnemyPrefabMapping mapping = enemyPrefabs.Find(m =>
            m.enemyTypeIdentifier == typeIdentifier);
            if (mapping == null || mapping.enemyPrefab == null)
            {
                Debug.LogError($"[EnemyFactory] Prefab not found for type: {typeIdentifier}");
                return null;
            }
            GameObject enemyInstance = Instantiate(mapping.enemyPrefab,
                position, Quaternion.identity, enemyContainer);
            enemyInstance.name = $"{typeIdentifier}_{Time.time}"; 
           
            EnemyController controller = enemyInstance.GetComponent<EnemyController>();
       
            return enemyInstance;
        }
    }
}
