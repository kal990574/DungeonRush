using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private ArenaBoundary _arenaBoundary;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _maxEnemies = 10;

    private float _lastSpawnTime;
    private int _activeEnemyCount;
    private IObjectPool<EnemyController> _enemyPool;
    private Transform _poolParent;

    private void Start()
    {
        _poolParent = new GameObject("EnemyPool").transform;
        _enemyPool = new ObjectPool<EnemyController>(
            createFunc: () => Instantiate(_enemyPrefab, _poolParent),
            actionOnGet: e => e.gameObject.SetActive(true),
            actionOnRelease: e => e.gameObject.SetActive(false),
            actionOnDestroy: e => Destroy(e.gameObject),
            defaultCapacity: 10,
            maxSize: 30);
    }

    private void Update()
    {
        if (Time.time < _lastSpawnTime + _spawnInterval) return;
        if (_activeEnemyCount >= _maxEnemies) return;

        SpawnEnemy();
        _lastSpawnTime = Time.time;
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPos = _arenaBoundary.GetRandomEdgePosition();
        EnemyController enemy = _enemyPool.Get();
        enemy.OnReturnRequested = HandleEnemyReturned;
        enemy.Activate(spawnPos);
        _activeEnemyCount++;
    }

    private void HandleEnemyReturned(EnemyController enemy)
    {
        _activeEnemyCount--;
        enemy.OnReturnRequested = null;
        _enemyPool.Release(enemy);
    }
}