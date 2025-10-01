public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyFactoryProvider factory;

    public void SpawnEnemy(Vector3 pos)
    {
        var enemy = factory.CreateEnemy(pos, Quaternion.identity);
        // Iniciar enemy
    }
}