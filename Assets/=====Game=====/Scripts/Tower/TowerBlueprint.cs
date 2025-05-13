using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public int cost;
    public int upgradeCost;

    public int GetSellAmount(bool isUpgraded)
    {
        return isUpgraded ? (cost + upgradeCost) / 2 : cost / 2;
    }
}