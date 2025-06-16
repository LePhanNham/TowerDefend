using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private int deathReward = 10;

    private float currentHealth;
    private Vector3 endPoint;
    private bool isDead = false;
    private float originalMoveSpeed;

    [Header("Reference")]
    [SerializeField] private Waypoint waypoint;
    //[SerializeField] private EnemyHealth _enemyHealth;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] public EnemyHealth _enemyHealth;

    [Header("Collections")]
    [SerializeField] public int DeathCoinReward = 5;

    public event Action<Enemy> OnDeath;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        waypoint = GetComponent<Waypoint>();
        _enemyHealth = GetComponent<EnemyHealth>();
        originalMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        if (waypoint != null && waypoint.Points.Length > 0)
        {
            endPoint = waypoint.GetWaypointPosition(waypoint.GetLengthPoint() - 1);
            currentHealth = maxHealth;
        }
    }

    private void Update()
    {
        if (isDead) return;

        MoveToEndPoint();
    }

    private void MoveToEndPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPoint, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endPoint) < 0.1f)
        {
            ReachEndPoint();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke(this);
        GameManager.Instance.AddMoney(deathReward);
        Destroy(gameObject);
    }

    private void ReachEndPoint()
    {
        GameManager.Instance.TakeDamage(damage);
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    private void Rotate()
    {
        if (endPoint.x > transform.position.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    public void SetWaypoint(Waypoint newWaypoint)
    {
        waypoint = newWaypoint;
        if (waypoint != null && waypoint.Points.Length > 0)
        {
            endPoint = waypoint.GetWaypointPosition(waypoint.GetLengthPoint() - 1);
        }
    }

    public void StopMovement()
    {
        moveSpeed = 0;
    }

    public void ResumeMovement()
    {
        moveSpeed = originalMoveSpeed;
    }

    public void Initialize(Vector3 endPoint, float healthMultiplier = 1f, float speedMultiplier = 1f)
    {
        this.endPoint = endPoint;
        maxHealth *= healthMultiplier;
        currentHealth = maxHealth;
        moveSpeed = originalMoveSpeed * speedMultiplier;
    }
}