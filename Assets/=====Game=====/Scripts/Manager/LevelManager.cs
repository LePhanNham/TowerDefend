using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{
    #region Level Settings
    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float timeBetweenEnemies = 1f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform endPoint;
    #endregion

    #region Wave Settings
    [Header("Wave Settings")]
    [SerializeField] private LevelData currentLevelData;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private bool isWaveActive = false;
    #endregion

    #region Level State
    public bool IsLevelComplete { get; private set; }
    public bool IsLevelFailed { get; private set; }
    public int EnemiesRemaining { get; private set; }
    public int TotalEnemiesInWave { get; private set; }
    #endregion

    #region UI References
    [Header("UI References")]
    [SerializeField] private GameObject levelCompleteUI;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private GameObject levelTransitionEffect;
    #endregion

    #region Events
    public static event Action<int> OnWaveStarted;
    public static event Action<int> OnWaveCompleted;
    public static event Action OnLevelComplete;
    public static event Action OnLevelFailed;
    public static event Action<int> OnEnemySpawned;
    public static event Action<int> OnEnemyDefeated;
    #endregion

    #region Save Data Keys
    private const string CURRENT_LEVEL_KEY = "CURRENT_LEVEL";
    private const string WAVE_PROGRESS_KEY = "WAVE_PROGRESS";
    #endregion

    public override void Awake()
    {
        base.Awake();
        LoadLevelProgress();
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RestartLevel);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }
    }

    private void Start()
    {
        Waypoint waypoint = FindObjectOfType<Waypoint>();
        if (waypoint != null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            endPoint = new GameObject("EndPoint").transform;
            
            // Set spawn point to first waypoint
            spawnPoint.position = waypoint.GetWaypointPosition(0);
            
            // Set end point to last waypoint
            endPoint.position = waypoint.GetWaypointPosition(waypoint.GetLengthPoint() - 1);
        }
        else
        {
            Debug.LogError("No Waypoint found in scene!");
            return;
        }
    }

    #region Level Management
    public void StartLevel(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is null!");
            return;
        }

        currentLevelData = levelData;
        currentWave = 0;
        IsLevelComplete = false;
        IsLevelFailed = false;
        EnemiesRemaining = 0;
        TotalEnemiesInWave = 0;

        // Initialize level through LevelController
        LevelController.Instance.InitializeLevel(levelData);
    }

    public void RestartLevel()
    {
        StartLevel(currentLevelData);
    }

    public void NextLevel()
    {
        if (currentLevel < maxLevel)
        {
            StartCoroutine(TransitionToNextLevel());
        }
        else
        {
            Debug.Log("You've completed all levels!");
            ShowGameCompleteUI();
        }
    }

    private IEnumerator TransitionToNextLevel()
    {
        if (levelTransitionEffect != null)
        {
            levelTransitionEffect.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        StartLevel(currentLevelData);

        if (levelTransitionEffect != null)
        {
            levelTransitionEffect.SetActive(false);
        }
    }

    private void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region Wave Management
    private void StartNextWave()
    {
        if (currentLevelData == null || currentWave >= currentLevelData.waves.Count)
        {
            CompleteLevel();
            return;
        }

        isWaveActive = true;
        WaveData wave = currentLevelData.waves[currentWave];
        TotalEnemiesInWave = CalculateTotalEnemiesInWave(wave);
        EnemiesRemaining = TotalEnemiesInWave;

        OnWaveStarted?.Invoke(currentWave + 1);
        StartCoroutine(SpawnWave(wave));
    }

    private int CalculateTotalEnemiesInWave(WaveData wave)
    {
        int total = 0;
        foreach (var enemySpawn in wave.enemies)
        {
            total += enemySpawn.count;
        }
        return total;
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var enemySpawn in wave.enemies)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                SpawnEnemy(enemySpawn.enemyPrefab, enemySpawn.healthMultiplier, enemySpawn.speedMultiplier);
                OnEnemySpawned?.Invoke(EnemiesRemaining);
                yield return new WaitForSeconds(enemySpawn.spawnDelay);
            }
        }

        isWaveActive = false;
        currentWave++;
        SaveLevelProgress();

        if (currentWave < currentLevelData.waves.Count)
        {
            yield return new WaitForSeconds(wave.waveDelay);
            StartNextWave();
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab, float healthMultiplier, float speedMultiplier)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.Initialize(endPoint.position, healthMultiplier, speedMultiplier);
                enemyComponent.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        EnemiesRemaining--;
        OnEnemyDefeated?.Invoke(EnemiesRemaining);

        if (EnemiesRemaining <= 0 && !isWaveActive)
        {
            OnWaveCompleted?.Invoke(currentWave);
        }
    }
    #endregion

    #region Level Progress
    private void CompleteLevel()
    {
        IsLevelComplete = true;
        OnLevelComplete?.Invoke();
        SaveLevelProgress();
        ShowLevelCompleteUI();
    }

    public void FailLevel()
    {
        IsLevelFailed = true;
        OnLevelFailed?.Invoke();
        ShowLevelFailedUI();
    }

    private void ShowLevelCompleteUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            levelCompleteText.text = $"Level {currentLevel} Complete!";
            rewardText.text = $"Rewards:\nMoney: +{currentLevelData.completionReward}";

            // Add rewards
            GameManager.Instance.AddMoney(currentLevelData.completionReward);

            // Show/hide next level button based on whether there are more levels
            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(currentLevel < maxLevel);
            }
        }
    }

    private void ShowLevelFailedUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            levelCompleteText.text = "Level Failed!";
            rewardText.text = "Try again!";
            
            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(false);
            }
        }
    }

    private void ShowGameCompleteUI()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
            levelCompleteText.text = "Congratulations!";
            rewardText.text = "You've completed all levels!";
            
            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(false);
            }
        }
    }

    private void SaveLevelProgress()
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, currentLevel);
        PlayerPrefs.SetInt(WAVE_PROGRESS_KEY, currentWave);
        PlayerPrefs.Save();
    }

    private void LoadLevelProgress()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
        currentWave = PlayerPrefs.GetInt(WAVE_PROGRESS_KEY, 0);
    }

    public void ClearLevelProgress()
    {
        PlayerPrefs.DeleteKey(CURRENT_LEVEL_KEY);
        PlayerPrefs.DeleteKey(WAVE_PROGRESS_KEY);
        PlayerPrefs.Save();
        LoadLevelProgress();
    }
    #endregion

    #region Utility Methods
    public bool IsWaveInProgress()
    {
        return isWaveActive || EnemiesRemaining > 0;
    }

    public float GetWaveProgress()
    {
        if (TotalEnemiesInWave == 0) return 0f;
        return 1f - ((float)EnemiesRemaining / TotalEnemiesInWave);
    }
    #endregion

    private void OnEnable()
    {
        LevelController.OnLevelStart += HandleLevelStart;
        LevelController.OnLevelPause += HandleLevelPause;
        LevelController.OnLevelResume += HandleLevelResume;
        LevelController.OnLevelEnd += HandleLevelEnd;
    }

    private void OnDisable()
    {
        LevelController.OnLevelStart -= HandleLevelStart;
        LevelController.OnLevelPause -= HandleLevelPause;
        LevelController.OnLevelResume -= HandleLevelResume;
        LevelController.OnLevelEnd -= HandleLevelEnd;
    }

    private void HandleLevelStart()
    {
        StartNextWave();
    }

    private void HandleLevelPause()
    {
        // Pause wave spawning and enemy movement
        isWaveActive = false;
    }

    private void HandleLevelResume()
    {
        // Resume wave spawning if there are remaining waves
        if (currentWave < currentLevelData.waves.Count)
        {
            isWaveActive = true;
        }
    }

    private void HandleLevelEnd()
    {
        // Clean up level
        isWaveActive = false;
        currentWave = 0;
    }
}
