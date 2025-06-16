using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Information")]
    public string levelName;
    public int levelNumber;
    public Sprite levelBackground;
    public string levelDescription;

    [Header("Level Settings")]
    public int startingMoney = 100;
    public int startingHealth = 100;
    public float timeBetweenWaves = 5f;
    public float timeBetweenEnemies = 1f;

    [Header("Wave Settings")]
    public List<WaveData> waves;

    [Header("Rewards")]
    public int completionReward;
    public List<RewardData> specialRewards;

    [Header("Unlock Requirements")]
    public int requiredStars;
    public int requiredLevel;
}

[Serializable]
public class RewardData
{
    public enum RewardType
    {
        Money,
        Health,
        SpecialItem,
        UnlockTower
    }

    public RewardType rewardType;
    public int amount;
    public string description;
    public Sprite rewardIcon;
}

[Serializable]
public class WaveData
{
    public string waveName;
    public List<EnemySpawnData> enemies;
    public float waveDelay = 5f;
    public bool isBossWave;
}

[Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnDelay = 1f;
    public float healthMultiplier = 1f;
    public float speedMultiplier = 1f;
}
