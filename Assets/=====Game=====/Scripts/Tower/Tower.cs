using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public List<Enemy> _enemy;

    public Enemy currentEnemyTarget;
    public TowerBlueprint blueprint;
    [SerializeField] public SpriteRenderer _spriteRenderer;
    public bool isUpgraded;

    private void Update()
    {
        GetCurrentEnemyTarget();
        RotateTowardsTarget();
    }

    private void RotateTowardsTarget()
    {
        if (currentEnemyTarget == null) return;
        else
        {
            Vector3 direction = currentEnemyTarget.transform.position - transform.position;
            float angle = Vector3.SignedAngle(transform.up, direction, transform.forward);
            transform.Rotate(0f,0f,angle);
        }
    }

    private void GetCurrentEnemyTarget()
    {
        _enemy.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);
        
        if (_enemy.Count <= 0)
        {
            currentEnemyTarget = null;
            return;
        }
        else
        {
            currentEnemyTarget = _enemy[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null && !_enemy.Contains(enemy))
            {
                _enemy.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (_enemy.Contains(enemy))
            {
                _enemy.Remove(enemy);
            }
        }
    }
}
