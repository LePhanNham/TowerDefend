using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    [SerializeField] protected Transform projectileSpawnPosition;
    [SerializeField] protected float delayBtwAttacks = 2f;
    [SerializeField] protected float damage = 10f;


    public float Damage { get; set; }
    public float DelayPerShot { get; set; }
    protected float nextAttackTime = 0f;
    public ObjectPooler _pooler;
    protected Tower _tower;
    public Projectile currentProjectileLoaded;
    public GameObject newInstance;
    private void Awake()
    {
        _pooler = GetComponent<ObjectPooler>();
        _tower = GetComponent<Tower>();
        Damage = damage;
        DelayPerShot = delayBtwAttacks;
    }
    private void Start()
    {
        _pooler.transform.position = projectileSpawnPosition.position;
    }
    protected virtual void Update()
    {

        if (Time.time >= nextAttackTime)
        {
            if (_tower.currentEnemyTarget != null && _tower.currentEnemyTarget._enemyHealth.curHealth > 0f)
            {
                LoadProjectile();
                currentProjectileLoaded.SetEnemy(_tower.currentEnemyTarget);

            }
            nextAttackTime = Time.time + DelayPerShot;
        }
    }

    protected virtual void LoadProjectile()
    {
        newInstance = _pooler.GetInstanceFromPool();
        newInstance.SetActive(true);
        currentProjectileLoaded = newInstance.GetComponent<Projectile>();
        currentProjectileLoaded.towerProjectile = this;
        currentProjectileLoaded.GetComponent<Projectile>().enabled = true;
        ResetProjectile();
        currentProjectileLoaded.Damage = Damage;
    }

    public void ResetProjectile()
    {
        newInstance.transform.position = projectileSpawnPosition.position;
    }
}
