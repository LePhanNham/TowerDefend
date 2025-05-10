using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public int playerHealth = 100;
    public int maxHealth = 100;
    public int money = 100;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI moneyText;
    public Image healBar;
    public GameObject gameOverUI;

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        UpdateUI();
    }

    public void PlayerTakeDamage(int damage = 1)
    {
        playerHealth -= damage;
        UpdateUI();

        if (playerHealth <= 0)
        {
            GameOver();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void UpdateUI()
    {
        healthText.text = playerHealth.ToString();
        moneyText.text = money.ToString();
        healBar.fillAmount = Mathf.Lerp(healBar.fillAmount, playerHealth / maxHealth, Time.deltaTime * 10f);
    }

    void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; 
    }
    public void ShowMessage(string message)
    {
    }
}