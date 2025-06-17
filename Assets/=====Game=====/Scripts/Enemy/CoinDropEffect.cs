using UnityEngine;
using System.Collections;
using TMPro;

public class CoinDropEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float dropForce = 5f;
    [SerializeField] private float moveToUI_Duration = 0.5f;
    [SerializeField] private float waitBeforeMove = 0.5f;
    [SerializeField] private int coinValue = 1;

    [Header("References")]
    [SerializeField] private Transform moneyTextTransform; // Vị trí của text hiển thị tiền

    private void Start()
    {
        if (moneyTextTransform == null)
        {
            // Tìm text hiển thị tiền trong scene
            GameObject moneyText = GameObject.FindGameObjectWithTag("MoneyText");
            if (moneyText != null)
            {
                moneyTextTransform = moneyText.transform;
            }
        }
    }

    public void DropCoins(Vector3 position, int amount)
    {
        StartCoroutine(SpawnCoins(position, amount));
    }

    private IEnumerator SpawnCoins(Vector3 position, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Tạo đồng xu
            GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
            
            // Thêm lực ngẫu nhiên để đồng xu rơi theo hướng khác nhau
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f)
                ).normalized;
                rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
            }

            // Đợi một chút trước khi di chuyển lên UI
            yield return new WaitForSeconds(waitBeforeMove);

            // Di chuyển đồng xu lên vị trí text tiền
            if (moneyTextTransform != null)
            {
                StartCoroutine(MoveCoinToUI(coin));
            }
            else
            {
                // Nếu không tìm thấy vị trí text tiền, chỉ cộng tiền
                GameManager.Instance.AddMoney(coinValue);
                Destroy(coin);
            }
        }
    }

    private IEnumerator MoveCoinToUI(GameObject coin)
    {
        Vector3 startPosition = coin.transform.position;
        Vector3 targetPosition = moneyTextTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveToUI_Duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveToUI_Duration;

            // Di chuyển đồng xu
            coin.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Thu nhỏ đồng xu
            coin.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }

        // Cộng tiền và xóa đồng xu
        GameManager.Instance.AddMoney(coinValue);
        Destroy(coin);
    }
} 