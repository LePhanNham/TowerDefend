using UnityEngine;
using System.Collections;
using TMPro;

public class CoinDropEffect : MonoBehaviour
{
    public static CoinDropEffect Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject coinTextPrefab; // Prefab chứa cả text và ảnh coin

    [Header("Settings")]
    [SerializeField] private float showDuration = 1f; // Thời gian hiển thị
    [SerializeField] private float floatDistance = 2f; // Tăng khoảng cách bay lên
    [SerializeField] private float fadeStartTime = 0.5f; // Thời điểm bắt đầu fade out
    [SerializeField] private float startYOffset = 0.5f; // Offset ban đầu so với vị trí enemy

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount, Vector3 position)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(amount);
            ShowCoinText(amount, position);
        }
    }

    private void ShowCoinText(int amount, Vector3 position)
    {
        if (coinTextPrefab == null) return;

        // Tạo vị trí bắt đầu cao hơn một chút so với enemy
        Vector3 startPos = position + new Vector3(0, startYOffset, 0);
        GameObject obj = Instantiate(coinTextPrefab, startPos, Quaternion.identity);
        obj.layer = LayerMask.NameToLayer("UI");

        // Set text từ amount
        TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = "+" + amount.ToString();
        }

        StartCoroutine(AnimateCoinText(obj));
    }

    private System.Collections.IEnumerator AnimateCoinText(GameObject obj)
    {
        float elapsed = 0f;
        Vector3 startPosition = obj.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up * floatDistance;

        // Lấy tất cả các component cần fade out
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        while (elapsed < showDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / showDuration;

            // Di chuyển lên trên với easing
            float smoothT = Mathf.SmoothStep(0, 1, t); // Thêm easing để chuyển động mượt hơn
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, smoothT);

            // Fade out
            if (elapsed > fadeStartTime)
            {
                float fadeT = (elapsed - fadeStartTime) / (showDuration - fadeStartTime);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
            }

            yield return null;
        }

        Destroy(obj);
    }
} 
