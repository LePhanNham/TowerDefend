using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private GameObject enemy;
    [SerializeField] private EnemyData enemyData;

    [Header("Delays")]
    [SerializeField] private float timeDelay;

    [Header("Present")]
    private float _spawnTime;
    private int _enemySpawned;
    private Waypoint _waypoint;
    public ObjectPooler _pooler;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _pooler = GetComponent<ObjectPooler>();
        if (_pooler == null)
        {
            Debug.LogError($"ObjectPooler component is missing on {gameObject.name}");
            enabled = false;
            return;
        }

        _waypoint = GetComponent<Waypoint>();
        if (_waypoint == null)
        {
            Debug.LogError($"Waypoint component is missing on {gameObject.name}");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (!enabled) return;

        _spawnTime -= Time.deltaTime;

        if (_spawnTime < 0)
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
        if (_pooler == null) return;

        GameObject newInstance = _pooler.GetInstanceFromPool();
        if (newInstance == null)
        {
            Debug.LogWarning("Failed to get enemy instance from pool");
            return;
        }

        newInstance.SetActive(true);

        Enemy enemyScript = newInstance.GetComponent<Enemy>();
        if (enemyScript == null)
        {
            Debug.LogWarning("Enemy component is missing on spawned instance");
            newInstance.SetActive(false);
            return;
        }

        enemyScript.enabled = true;
        
        SpriteRenderer spriteRenderer = newInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 12;
        }

        newInstance.transform.position = _pooler._poolContainer.transform.position;
        enemyScript.SetWaypoint(_waypoint);

        // Gán EnemyData cho enemy
        if (enemyData != null)
        {
            enemyScript.Initialize(_pooler._poolContainer.transform.position, 1f, 1f, enemyData);
            Debug.Log($"Spawned enemy with gold value: {enemyData.gold}");
        }
        else
        {
            Debug.LogWarning("EnemyData is not assigned in EnemySpawner!");
            enemyScript.Initialize(_pooler._poolContainer.transform.position);
        }
    }
}
