using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public int id;
    public List<EnemyData> enemies;
    public List<int> enemyCount;
    public float timeDelay;
}
