using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
    }

    private void Start()
    {
        if (spawnPoint == null || endPoint == null)
        {
            Debug.LogError("Spawn point or end point not set in LevelManager!");
            return;
        }
    }

    #region Level Management
    public void StartLevel(int level)
    {
        if (level < 1 || level > maxLevel)
        {
            Debug.LogError($"Invalid level number: {level}");
            return;
        }

        currentLevel = level;
        currentWave = 0;
        IsLevelComplete = false;
        IsLevelFailed = false;
        EnemiesRemaining = 0;
        TotalEnemiesInWave = 0;

        // TODO: Load level data based on level number
        // currentLevelData = Resources.Load<LevelData>($"Levels/Level_{level}");

        SaveLevelProgress();
        StartNextWave();
    }

    public void RestartLevel()
    {
        StartLevel(currentLevel);
    }

    public void NextLevel()
    {
        if (currentLevel < maxLevel)
        {
            StartLevel(currentLevel + 1);
        }
        else
        {
            Debug.Log("You've completed all levels!");
            // TODO: Implement game completion logic
        }
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
    }

    public void FailLevel()
    {
        IsLevelFailed = true;
        OnLevelFailed?.Invoke();
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
}
