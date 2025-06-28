using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private EnemyData enemyData;

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
    private Collider2D _collider;

    [Header("Collections")]
    [SerializeField] public int DeathCoinReward = 5;

    [Header("References")]
    [SerializeField] private CoinDropEffect coinDropEffect;

    public event Action<Enemy> OnDeath;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyHealth = GetComponent<EnemyHealth>();
        originalMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _collider = GetComponent<Collider2D>();
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

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log($"Enemy died at position {transform.position}");
        Debug.Log($"CoinDropEffect.Instance: {(CoinDropEffect.Instance != null ? "Exists" : "Null")}");
        Debug.Log($"enemyData: {(enemyData != null ? "Exists" : "Null")}");
        if (enemyData != null)
        {
            Debug.Log($"enemyData.gold: {enemyData.gold}");
        }
        
        // Tắt collider và movement
        if (_collider != null) _collider.enabled = false;
        enabled = false;

        // Thêm tiền trực tiếp
        if (enemyData != null)
        {
            int goldAmount = Mathf.RoundToInt(enemyData.gold);
            CoinDropEffect.Instance.AddCoins(goldAmount, transform.position);
        }

        // Hiệu ứng chớp chớp rồi biến mất
        StartCoroutine(FlashAndDisappear());
        
        // Thông báo enemy chết
        OnDeath?.Invoke(this);
    }

    private IEnumerator FlashAndDisappear()
    {
        if (_spriteRenderer == null) yield break;

        // Số lần chớp
        int flashCount = 3;
        float flashDuration = 0.1f;
        float waitBetweenFlashes = 0.1f;

        // Lưu màu gốc
        Color originalColor = _spriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            // Chớp trắng
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);

            // Trở về màu gốc
            _spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(waitBetweenFlashes);
        }

        // Biến mất
        gameObject.SetActive(false);
        
        // Reset sau khi biến mất
        Reset();
    }

    private void ReachEndPoint()
    {
        if (isDead) return; // Đảm bảo chỉ gọi 1 lần
        isDead = true;      // Đánh dấu enemy đã xử lý

        // Trừ máu người chơi
        GameManager.Instance.TakeDamage(damage);

        // Hiệu ứng khi enemy đến điểm cuối (nếu có)
        if (TryGetComponent<EnemyAnimations>(out var animations))
        {
            StartCoroutine(PlayReachEndPointEffect());
        }

        // Thông báo số máu bị mất
        ShowDamageText(damage);

        // Phát âm thanh
        AudioManager.Instance.PlaySound("enemy_reach_end", 1f);

        // Gọi sự kiện enemy chết
        OnDeath?.Invoke(this);

        // Tắt enemy để trả về pool, tránh spam
        gameObject.SetActive(false);
    }

    private IEnumerator PlayReachEndPointEffect()
    {
        // Dừng di chuyển
        StopMovement();

        // Hiệu ứng biến mất
        float fadeTime = 0.5f;
        float elapsedTime = 0f;
        Color originalColor = _spriteRenderer.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Vô hiệu hóa enemy
        gameObject.SetActive(false);
    }

    private void ShowDamageText(int damage)
    {
        // Tạo text damage
        GameObject damageText = new GameObject("DamageText");
        damageText.transform.position = transform.position;
        TextMeshPro textMesh = damageText.AddComponent<TextMeshPro>();
        textMesh.text = $"-{damage}";
        textMesh.color = Color.red;
        textMesh.fontSize = 5;
        textMesh.alignment = TextAlignmentOptions.Center;

        // Animation text bay lên và biến mất
        StartCoroutine(AnimateDamageText(textMesh));
    }

    private IEnumerator AnimateDamageText(TextMeshPro textMesh)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPos = textMesh.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Di chuyển text lên trên
            textMesh.transform.position = Vector3.Lerp(startPos, endPos, t);

            // Fade out text
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            textMesh.color = color;

            yield return null;
        }

        Destroy(textMesh.gameObject);
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

    public void Initialize(Vector3 endPoint, float healthMultiplier = 1f, float speedMultiplier = 1f, EnemyData data = null)
    {
        Reset(); // Reset trước khi khởi tạo lại
        this.endPoint = endPoint;
        maxHealth *= healthMultiplier;
        currentHealth = maxHealth;
        moveSpeed = originalMoveSpeed * speedMultiplier;
        isDead = false;
        currentWaypointIndex = 0;
        enabled = true;

        // Gán EnemyData nếu được cung cấp
        if (data != null)
        {
            enemyData = data;
            Debug.Log($"Enemy initialized with data, gold value: {enemyData.gold}");
        }
    }

    private void Reset()
    {
        // Reset các biến trạng thái
        isDead = false;
        currentWaypointIndex = 0;
        currentHealth = maxHealth;
        moveSpeed = originalMoveSpeed;

        // Reset sprite renderer
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white;
            _spriteRenderer.flipX = false;
        }

        // Reset health bar
        if (_enemyHealth != null)
        {
            _enemyHealth.ResetHealth();
        }

        // Reset collider
        if (_collider != null)
        {
            _collider.enabled = true;
        }

        // Reset vị trí và rotation
        transform.rotation = Quaternion.identity;
        
        // Bật lại component
        enabled = true;
    }

    private void OnDisable()
    {
        // Không reset khi enemy bị vô hiệu hóa (trả về pool)
        // Reset sẽ được gọi sau khi hiệu ứng chớp chớp kết thúc
    }
}