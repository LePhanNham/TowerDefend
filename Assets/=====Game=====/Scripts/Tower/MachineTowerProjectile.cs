using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineTowerProjectile : TowerProjectile
{
    [SerializeField] private bool isDualMachine;
    [SerializeField] private float spreadRange;

    protected override void Update()
    {
        if (Time.time > nextAttackTime)
        {
            if (_tower.currentEnemyTarget != null)
            {
                Vector3 dirToTarget = _tower.currentEnemyTarget.transform.position - transform.position;
                FireProjectile();
            }
            nextAttackTime = Time.time + delayBtwAttacks;
        }
    }


    protected override void LoadProjectile()
    {

    }
    private void FireProjectile()
    {
        GameObject instance = _pooler.GetInstanceFromPool();
        instance.transform.position = projectileSpawnPosition.position;
    }
}
