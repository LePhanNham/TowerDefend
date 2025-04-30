using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] public List<LevelData> levels; 
    [SerializeField] private int level;
    [SerializeField] private List<EnemyData> enemy;
    [SerializeField] private List<int> enemyCount;
    [SerializeField] private float timeDelay;
    [SerializeField] private List<ObjectPooler> objectPoolers;
    public int currentWave { get; set; }
    private void Start()
    {
    }
    public void SetLevel(int level, List<EnemyData> enemy, List<int> enemyCount,float timeDelay)
    {
        this.level = level;
        this.enemy = enemy;
        this.enemyCount = enemyCount;
        this.timeDelay = timeDelay;
    }
    public bool IsAlive()
    {
        if (GameManager.Instance.playerHealth>0) return true;
        return false;
    }


    public bool CheckNextLevel()
    {
        if (enemyCount[enemyCount.Count - 1] == 0 && IsAlive()) 
        {
            return true;
        }
        return false;
    }
    public void NextLevel()
    {
        if (CheckNextLevel())
        {
            level++;
            SetLevel(level, levels[level].enemies, levels[level].enemyCount, timeDelay);
        }
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
}
