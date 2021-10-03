using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnDelay = 2;
    [SerializeField] private float levelTimer;
    [SerializeField] private GameObject enemy;
    [SerializeField] private float baseEnemySpawnRate = 0.5f;
    [SerializeField] private GameObject boss;
    [SerializeField] private float bossSpawnRate = 0.1f;

    private Bounds _spawnBounds;
    private float _timeAfterLastSpawn;
    private float _currentTime = 0;

    void Start()
    {
        _spawnBounds = GetComponent<Collider>().bounds;
    }

    void Update()
    {
        _currentTime += Time.deltaTime;
        _timeAfterLastSpawn += Time.deltaTime;
        
        if (_timeAfterLastSpawn >= spawnDelay)
        {
            if (Random.Range(0f, 1f) < baseEnemySpawnRate)
            {
                _timeAfterLastSpawn = 0;
                var pos = GetRandomPosition(_spawnBounds.min, _spawnBounds.max);
                Instantiate(enemy, pos, Quaternion.identity);
                if (Random.Range(0f, 1f) < baseEnemySpawnRate / 2)
                {
                    pos = GetRandomPosition(_spawnBounds.min, _spawnBounds.max);
                    Instantiate(enemy, pos, Quaternion.identity); 
                }
                if (Random.Range(0f, 1f) < baseEnemySpawnRate / 4)
                {
                    pos = GetRandomPosition(_spawnBounds.min, _spawnBounds.max);
                    Instantiate(enemy, pos, Quaternion.identity); 
                }
            }

            if (Random.Range(0f, 1f) < bossSpawnRate)
            {
                _timeAfterLastSpawn = 0;
                var pos = GetRandomPosition(_spawnBounds.min, _spawnBounds.max);
                Instantiate(boss, pos, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomPosition(Vector3 min, Vector3 max)
        => new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z));
}