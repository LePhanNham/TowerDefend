using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level", menuName = "Tower Defense/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int startingMoney = 100;
    public int startingHealth = 100;
    public List<WaveData> waves;
    public int completionReward;
}

[System.Serializable]
public class WaveData
{
    public List<EnemySpawnData> enemies;
    public float waveDelay = 5f;
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnDelay = 1f;
}
