using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    private Vector3 currentPointPosition ;
    private Vector3 _lastPointPosition;

    private int _currenPointIndex = 0;

    [Header("Reference")]
    [SerializeField] private Waypoint waypoint;
    //[SerializeField] private EnemyHealth _enemyHealth;

    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] public EnemyHealth _enemyHealth;

    public event Action OnEndReached;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        waypoint = GetComponent<Waypoint>();
        _enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        if (waypoint != null && waypoint.Points.Length > 0)
        {
            currentPointPosition = waypoint.GetWaypointPosition(_currenPointIndex);

        }
    }

    private void Update()
    {
        if (waypoint == null || waypoint.Points.Length == 0)
            return;

        Move();
        Rotate();
        if (CurrentPointPositionReached())
        {
            UpdateCurrentPointIndex();
        }
    }

    private bool CurrentPointPositionReached()
    {
        float distance = (transform.position - currentPointPosition).magnitude;
        if (distance < 0.1f)
        {
            _lastPointPosition = transform.position;
            return true;
        }
        return false;
    }

    private void UpdateCurrentPointIndex()
    {
        int lastWaypointIndex = waypoint.GetLengthPoint() - 1;
        if (_currenPointIndex < lastWaypointIndex)
        {
            _currenPointIndex++;
            currentPointPosition = waypoint.GetWaypointPosition(_currenPointIndex);
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        OnEndReached?.Invoke();
        _enemyHealth.ResetHealth();
        waypoint.GetComponentInParent<EnemySpawner>()._pooler.ReturnToPool(gameObject);
    }

    private void Rotate()
    {
        if (currentPointPosition.x > _lastPointPosition.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPointPosition, moveSpeed * Time.deltaTime);
    }

    public void SetWaypoint(Waypoint newWaypoint)
    {
        waypoint = newWaypoint;
        if (waypoint != null && waypoint.Points.Length > 0)
        {
            _currenPointIndex = 0;
            currentPointPosition = waypoint.GetWaypointPosition(_currenPointIndex);
        }
    }

    public void StopMovement()
    {
        moveSpeed = 0;
    }
    public void ResumeMovement()
    {
        moveSpeed = 2;
    }
}