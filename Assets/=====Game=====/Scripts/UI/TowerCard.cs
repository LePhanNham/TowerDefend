using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : MonoBehaviour
{
    public static Action<TowerSettings> OnPlaceTower;
    private Image towerImage;
    private TextMeshProUGUI towerCostText;

    public TowerSettings towerloaded{ get; set; }
    public void SetUpTowerBtn(TowerSettings towerSettings)
    {
        towerloaded = towerSettings;
        towerImage.sprite = towerSettings.towerIcon;
        towerCostText.text = towerSettings.cost.ToString();
    }

    public void ToPlaceTower()
    {
        if (GameManager.Instance.money>=towerloaded.cost)
        {
            GameManager.Instance.SpendMoney(towerloaded.cost);
            OnPlaceTower?.Invoke(towerloaded);
        }
    }
}
