using System;
using System.Collections;
using UnityEngine;

public class UpgradeTower : MonoBehaviour
{
    [SerializeField] private int upgradeInitialCost;
    [SerializeField] private int upgradeCostIncremental;
    [SerializeField] private float damageIncremental;
    [SerializeField] private float delayReduce;
    [Header("Sell")]
    [Range(0, 1)]
    [SerializeField] private float sellPert;
    public float SellPerc { get; set; }
    public int UpgradeCost { get; set; }
    public int Level { get; set; }

    private TowerProjectile towerProjectile;

    private void Start()
    {
        towerProjectile = GetComponent<TowerProjectile>();
        UpgradeCost = upgradeInitialCost;
        SellPerc = sellPert;
        Level = 1;

    }

    //public void UpgradeTower()
    //{
    //    if (CurrencySystem.Instance.TotalCoins >= UpgradeCost)
    //    {
    //        towerProjectile.Damage += damageIncremental;
    //        towerProjectile.DelayPerShot -= delayReduce;
    //        UpdateUpgrade();
    //    }

    //}

    private void UpdateUpgrade()
    {
        throw new NotImplementedException();
    }
}
    //public int GetSellValue()

    //    int sellValue = Mathf.RoundToInt(UpgradeCost * SellPerc);
    //    return sellvalue;
    //}
