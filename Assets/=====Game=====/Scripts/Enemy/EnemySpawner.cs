using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;
    [SerializeField] private GameObject enemy;

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
        _waypoint = GetComponent<Waypoint>();
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
        newInstance.GetComponent<Enemy>().enabled = true;
        newInstance.GetComponent<SpriteRenderer>().sortingOrder = 2;
        Enemy enemyScript = newInstance.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            newInstance.transform.position = _pooler._poolContainer.transform.position;
            enemyScript.SetWaypoint(_waypoint);
        }
    }
}
