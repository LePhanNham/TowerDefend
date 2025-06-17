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
    private int currentWaypointIndex = 0;

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
        _enemyHealth = GetComponent<EnemyHealth>();
        originalMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        if (waypoint != null && waypoint.Points != null && waypoint.Points.Length > 0)
        {
            currentWaypointIndex = 0;
            endPoint = waypoint.GetWaypointPosition(currentWaypointIndex);
            currentHealth = maxHealth;
        }
        else
        {
            Debug.LogError("Waypoint hoặc Waypoint.Points là null/rỗng trong Enemy.Start! Enemy sẽ không di chuyển.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (isDead || waypoint == null || !enabled) return;

        MoveToEndPoint();
        Rotate();
    }

    private void MoveToEndPoint()
    {
        if (waypoint == null || waypoint.Points == null || waypoint.Points.Length == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, endPoint, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endPoint) < 0.1f)
        {
            if (currentWaypointIndex < waypoint.GetLengthPoint() - 1)
            {
                currentWaypointIndex++;
                endPoint = waypoint.GetWaypointPosition(currentWaypointIndex);
            }
            else
            {
                ReachEndPoint();
            }
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
    }

    private void ReachEndPoint()
    {
        GameManager.Instance.TakeDamage(damage);
        OnDeath?.Invoke(this);
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    private void Rotate()
    {
        if (_spriteRenderer == null) return;

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
        if (waypoint != null && waypoint.Points != null && waypoint.Points.Length > 0)
        {
            currentWaypointIndex = 0;
            endPoint = waypoint.GetWaypointPosition(currentWaypointIndex);
            enabled = true;
        }
        else
        {
            Debug.LogWarning("SetWaypoint được gọi với Waypoint null hoặc rỗng. Enemy sẽ không di chuyển.");
            enabled = false;
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
        isDead = false;
        currentWaypointIndex = 0;
        enabled = true;
    }
}