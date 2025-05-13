using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCard : MonoBehaviour
{
    public static Action<TowerSettings> OnPlaceTower;
    public Image towerImage;
    public TextMeshProUGUI towerCostText;

    public TowerSettings towerloaded{ get; set; }

    private void Awake()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SetUIShop);
    }
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

    public void SetUIShop()
    {
        ShopUIManager.Instance.SetUnActiveShop();
        BuildManager.Instance.SelectTowerToBuild(towerloaded.towerPrefabs.GetComponent<Tower>());
    }
}
