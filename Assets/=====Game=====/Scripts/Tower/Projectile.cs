using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Action<Enemy, float> onEnemyHit;

    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.1f;

    public TowerProjectile towerProjectile { get; set; }
    public float Damage { get; set; }

    protected Enemy targetEnemy;

    protected virtual void Update()
    {
        if (targetEnemy != null && targetEnemy._enemyHealth.curHealth > 0)
        {

            MoveProjectile();
            RotateProjectile();
        }
    }

    private void RotateProjectile()
    {
        Vector3 enemyPos = targetEnemy.transform.position-transform.position;
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        transform.Rotate(0f, 0f, angle);
    }

    private void MoveProjectile()
    {

        transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, moveSpeed * Time.deltaTime);
        float distance = (transform.position - targetEnemy.transform.position).magnitude;
        if (distance < minDistanceToDealDamage)
        {
            onEnemyHit?.Invoke(targetEnemy, Damage);
            targetEnemy._enemyHealth.DealDamage(Damage);
            towerProjectile._pooler.ReturnToPool(towerProjectile.newInstance);
        }
    }

    //public void DealDame(Enemy enemy,float damage)
    //{
    //    enemy._enemyHealth.DealDamage(damage);

    //}
    public void SetEnemy(Enemy enemy)
    {
        targetEnemy = enemy;
    }


}
