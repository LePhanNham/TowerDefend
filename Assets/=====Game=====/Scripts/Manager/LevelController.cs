using UnityEngine;
using System;
using System.Collections;

public class LevelController : Singleton<LevelController>
{
    [Header("Level Settings")]
    [SerializeField] private LevelData currentLevelData;
    [SerializeField] private float countdownTime = 3f;
    [SerializeField] private GameObject countdownUI;
    [SerializeField] private TMPro.TextMeshProUGUI countdownText;

    // Level State
    public bool IsLevelStarted { get; private set; }
    public bool IsLevelPaused { get; private set; }
    public float LevelTime { get; private set; }

    // Events
    public static event Action OnLevelStart;
    public static event Action OnLevelPause;
    public static event Action OnLevelResume;
    public static event Action OnLevelEnd;

    private void Start()
    {
        if (countdownUI != null)
            countdownUI.SetActive(false);
    }

    public void InitializeLevel(LevelData levelData)
    {
        currentLevelData = levelData;
        IsLevelStarted = false;
        IsLevelPaused = false;
        LevelTime = 0f;

        // Initialize game state
        GameManager.Instance.InitializeLevel(levelData.startingMoney, levelData.startingHealth);
        
        // Start countdown
        StartCoroutine(StartLevelCountdown());
    }

    private IEnumerator StartLevelCountdown()
    {
        if (countdownUI != null)
            countdownUI.SetActive(true);

        float remainingTime = countdownTime;
        while (remainingTime > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(remainingTime).ToString();
            
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        if (countdownUI != null)
            countdownUI.SetActive(false);

        StartLevel();
    }

    private void StartLevel()
    {
        IsLevelStarted = true;
        IsLevelPaused = false;
        LevelTime = 0f;
        
        // Start spawning enemies
        if (LevelManager.Instance != null)
            LevelManager.Instance.StartLevel(currentLevelData);
        
        OnLevelStart?.Invoke();
    }

    private void Update()
    {
        if (IsLevelStarted && !IsLevelPaused)
        {
            LevelTime += Time.deltaTime;
        }
    }

    public void PauseLevel()
    {
        if (!IsLevelStarted) return;
        
        IsLevelPaused = true;
        Time.timeScale = 0f;
        OnLevelPause?.Invoke();
    }

    public void ResumeLevel()
    {
        if (!IsLevelStarted) return;
        
        IsLevelPaused = false;
        Time.timeScale = 1f;
        OnLevelResume?.Invoke();
    }

    public void EndLevel()
    {
        IsLevelStarted = false;
        IsLevelPaused = false;
        OnLevelEnd?.Invoke();
    }
} 