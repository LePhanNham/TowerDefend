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
    [SerializeField] private Transform spawnPoint; // Assign via Inspector
    [SerializeField] private Transform endPoint;   // Assign via Inspector
    #endregion

    #region Wave Settings
    [Header("Wave Settings")]
    [SerializeField] private LevelData currentLevelData;
    [SerializeField] private int currentWave = 0;
    private bool isWaveActive = false;
    #endregion

    #region Level State
    public bool IsLevelComplete { get; private set; }
    public bool IsLevelFailed { get; private set; }
    public int EnemiesRemaining { get; private set; }
    public int TotalEnemiesInWave { get; private set; }
    #endregion

    #region Events
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnLevelComplete;
    public event Action OnLevelFailed;
    public event Action<int> OnEnemySpawned;
    public event Action<int> OnEnemyDefeated;
    #endregion

    #region Save Data Keys
    private const string CURRENT_LEVEL_KEY = "CURRENT_LEVEL";
    private const string WAVE_PROGRESS_KEY = "WAVE_PROGRESS";
    #endregion

    public int CurrentLevel => currentLevel;
    public int MaxLevel => maxLevel;
    public LevelData CurrentLevelData => currentLevelData;

    public override void Awake()
    {
        base.Awake();
        LoadLevelProgress();
    }

    private void Start()
    {
        if (spawnPoint == null || endPoint == null)
        {
            Debug.LogError("SpawnPoint and EndPoint must be assigned in the Inspector!");
            enabled = false;
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
        LevelController.Instance.InitializeLevel(levelData);
    }

    public void RestartLevel() => StartLevel(currentLevelData);

    public void NextLevel()
    {
        if (currentLevel < maxLevel)
            StartCoroutine(TransitionToNextLevel());
        else
            OnLevelComplete?.Invoke();
    }

    private IEnumerator TransitionToNextLevel()
    {
        yield return new WaitForSeconds(1f);
        StartLevel(currentLevelData);
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
        var wave = currentLevelData.waves[currentWave];
        TotalEnemiesInWave = CalculateTotalEnemiesInWave(wave);
        EnemiesRemaining = TotalEnemiesInWave;
        OnWaveStarted?.Invoke(currentWave + 1);
        StartCoroutine(SpawnWave(wave));
    }

    private int CalculateTotalEnemiesInWave(WaveData wave)
    {
        int total = 0;
        foreach (var enemySpawn in wave.enemies)
            total += enemySpawn.count;
        return total;
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var enemySpawn in wave.enemies)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                SpawnEnemy(enemySpawn.enemyPrefab);
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

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            var enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            var enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.Initialize(endPoint.position);
                enemyComponent.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        EnemiesRemaining--;
        OnEnemyDefeated?.Invoke(EnemiesRemaining);
        if (EnemiesRemaining <= 0 && !isWaveActive)
            OnWaveCompleted?.Invoke(currentWave);
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
    public bool IsWaveInProgress() => isWaveActive || EnemiesRemaining > 0;
    public float GetWaveProgress() => TotalEnemiesInWave == 0 ? 0f : 1f - ((float)EnemiesRemaining / TotalEnemiesInWave);
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

    private void HandleLevelStart() => StartNextWave();
    private void HandleLevelPause() => isWaveActive = false;
    private void HandleLevelResume() { if (currentWave < currentLevelData.waves.Count) isWaveActive = true; }
    private void HandleLevelEnd() { isWaveActive = false; currentWave = 0; }
}
