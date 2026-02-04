using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private ArenaBoundary _arenaBoundary;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _maxEnemies = 10;

    private float _lastSpawnTime;
    private int _activeEnemyCount;

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
        GameObject enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
        _activeEnemyCount++;

        var health = enemy.GetComponent<HealthComponent>();
        health.OnDied += () => _activeEnemyCount--;
    }
}