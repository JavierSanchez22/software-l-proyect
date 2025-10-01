using UnityEngine;

public class EnemyFactoryProvider : MonoBehaviour, IEnemyFactory
{
    [SerializeField] private GameObject enemyPrefab;

    public EnemyModel CreateEnemy(Vector3 position, Quaternion rotation)
    {
        var obj = Instantiate(enemyPrefab, position, rotation);
        return obj.GetComponent<EnemyModel>();
    }
}