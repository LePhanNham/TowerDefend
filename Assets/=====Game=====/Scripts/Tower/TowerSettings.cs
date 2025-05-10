using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerSettings", menuName = "ScriptableObjects/TowerSettings", order = 1)]
public class TowerSettings : ScriptableObject
{
    public GameObject towerPrefabs;
    public int cost;
    public Sprite towerIcon;
}
