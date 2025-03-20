using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthContainer : MonoBehaviour
{
    [SerializeField] private Image _fillAmountImage;
    public Image fillAmountImage => _fillAmountImage;
}
