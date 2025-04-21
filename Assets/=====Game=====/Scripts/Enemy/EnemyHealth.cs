using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public static Action<Enemy> OnEnemyKilled;
    public static Action<Enemy> OnEnemyHit;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform healthBarPosition;
    [SerializeField] private float initHealth = 40f;
    [SerializeField] private float maxHealth = 40f;

    [SerializeField] public float curHealth { get; set; }

    private Image _healthBar;
    private Enemy _enemy;

    private void Start()
    {
        CreateHealthBar();
        curHealth = initHealth;
        _enemy = GetComponent<Enemy>();
    }



    private void Update()
    {

        _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount, curHealth / maxHealth, Time.deltaTime * 10f);
    }
    private void CreateHealthBar()
    {
        GameObject newBar = Instantiate(healthBarPrefab,healthBarPosition.position,Quaternion.identity);
        newBar.transform.SetParent(transform.GetChild(0));
        EnemyHealthContainer container = newBar.GetComponent<EnemyHealthContainer>();
        _healthBar = container.fillAmountImage;
    }

    public void ResetHealth()
    {
        curHealth = maxHealth;
    }
    public void DealDamage(float damageReceived)
    {
        curHealth -= damageReceived;
        if (curHealth <= 0)
        {
            curHealth = 0;
            Die();
        }
        else
        {
            OnEnemyHit?.Invoke(_enemy);
        }
    }

    private void Die()
    {
        OnEnemyKilled?.Invoke(_enemy);

    }
}

