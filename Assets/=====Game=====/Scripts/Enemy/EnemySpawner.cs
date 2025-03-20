using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private GameObject enemy;

    [Header("Delays")]
    [SerializeField] private float timeDelay;

    [Header("Present")]
    private float _spawnTime;
    private int _enemySpawned;

    ObjectPooler _pooler;
    private void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
    }

    private void Update()
    {
        _spawnTime -=Time.deltaTime;

        if (_spawnTime < 0 )
        {
            _spawnTime = timeDelay;
            if (_enemySpawned < enemyCount)
            {
                _enemySpawned++;
                SpawnEnemy();
            }
        }
    }
    public void SpawnEnemy()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();
        newInstance.SetActive(true);
    }
}
