using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public SpriteRenderer spriteRenderer;
    public float health;
    public float moveSpeed;
    public float damage;
    public float gold;
}
