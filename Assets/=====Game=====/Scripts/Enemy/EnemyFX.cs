using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFX : MonoBehaviour
{
    [SerializeField] private Transform textDamageSpawnPosition;
    private Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();
    }


    public void EnemyHit(Enemy enemy, float damage)
    {
        Debug.Log("Hurt");
        if (enemy == _enemy)
        {

        }
    }
}
