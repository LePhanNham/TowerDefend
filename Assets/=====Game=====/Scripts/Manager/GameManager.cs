using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : Singleton<GameManager>
{
    #region Game State
    public bool IsGameStarted { get; private set; }
    public bool IsGamePaused { get; private set; }
    public bool IsGameOver { get; private set; }
    #endregion

    #region Player Stats
    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int initialMoney = 100;
    private int currentHealth;
    private int currentMoney;

    public int CurrentHealth => currentHealth;
    public int CurrentMoney => currentMoney;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    #endregion

    #region UI References
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseMenuUI;
    #endregion

    #region Events
    public static event Action<int> OnMoneyChanged;
    public static event Action<int> OnHealthChanged;
    public static event Action OnGameOver;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    #endregion

    #region Save Data Keys
    private const string MONEY_SAVE_KEY = "PLAYER_MONEY";
    private const string HEALTH_SAVE_KEY = "PLAYER_HEALTH";
    #endregion

    public override void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    private void Start()
    {
        LoadGameData();
        UpdateUI();
    }

    private void InitializeGame()
    {
        currentHealth = maxHealth;
        currentMoney = initialMoney;
        IsGameStarted = false;
        IsGamePaused = false;
        IsGameOver = false;
    }

    #region Level Management
    public void InitializeLevel(int startingMoney, int startingHealth)
    {
        currentMoney = startingMoney;
        currentHealth = startingHealth;
        maxHealth = startingHealth;
        IsGameStarted = true;
        IsGamePaused = false;
        IsGameOver = false;
        
        SaveGameData();
        UpdateUI();
    }
    #endregion

    #region Money Management
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            SaveMoney();
            OnMoneyChanged?.Invoke(currentMoney);
            UpdateUI();
            return true;
        }
        ShowMessage("Không đủ tiền!");
        return false;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        SaveMoney();
        OnMoneyChanged?.Invoke(currentMoney);
        UpdateUI();
    }
    #endregion

    #region Health Management
    public void TakeDamage(int damage = 1)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        SaveHealth();
        OnHealthChanged?.Invoke(currentHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        SaveHealth();
        OnHealthChanged?.Invoke(currentHealth);
        UpdateUI();
    }
    #endregion

    #region Game State Management
    public void StartGame()
    {
        if (!IsGameStarted)
        {
            IsGameStarted = true;
            Time.timeScale = 1f;
        }
    }

    public void PauseGame()
    {
        if (!IsGamePaused)
        {
            IsGamePaused = true;
            Time.timeScale = 0f;
            pauseMenuUI?.SetActive(true);
            OnGamePaused?.Invoke();
        }
    }

    public void ResumeGame()
    {
        if (IsGamePaused)
        {
            IsGamePaused = false;
            Time.timeScale = 1f;
            pauseMenuUI?.SetActive(false);
            OnGameResumed?.Invoke();
        }
    }

    private void GameOver()
    {
        IsGameOver = true;
        gameOverUI?.SetActive(true);
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        InitializeGame();
        LoadGameData();
        UpdateUI();
        gameOverUI?.SetActive(false);
        Time.timeScale = 1f;
    }
    #endregion

    #region Save/Load System
    private void SaveMoney()
    {
        PlayerPrefs.SetInt(MONEY_SAVE_KEY, currentMoney);
        PlayerPrefs.Save();
    }

    private void SaveHealth()
    {
        PlayerPrefs.SetInt(HEALTH_SAVE_KEY, currentHealth);
        PlayerPrefs.Save();
    }

    private void SaveGameData()
    {
        SaveMoney();
        SaveHealth();
    }

    private void LoadGameData()
    {
        currentMoney = PlayerPrefs.GetInt(MONEY_SAVE_KEY, initialMoney);
        currentHealth = PlayerPrefs.GetInt(HEALTH_SAVE_KEY, maxHealth);
    }
    #endregion

    #region UI Updates
    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
        
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
        
        if (healthBar != null)
            healthBar.fillAmount = HealthPercentage;
    }
    #endregion

    #region Utility Methods
    public void ShowMessage(string message)
    {
        // TODO: Implement proper message display system
        Debug.Log(message);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteKey(MONEY_SAVE_KEY);
        PlayerPrefs.DeleteKey(HEALTH_SAVE_KEY);
        PlayerPrefs.Save();
        LoadGameData();
        UpdateUI();
    }
    #endregion
}