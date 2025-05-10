using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Vector3 positionOffset;

    private GameObject tower;
    private Renderer rend;
    private Color startColor;
    private bool isUpgraded = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    void OnMouseDown()
    {
        if (BuildManager.Instance.GetTowerToBuild() == null)
        {
            BuildManager.Instance.SelectNode(this);
            return;
        }

        if (tower != null)
        {
            Debug.Log("Can't build there! - TODO: Display on screen.");
            return;
        }

        BuildTower(BuildManager.Instance.GetTowerToBuild());
    }

    void BuildTower(TowerBlueprint blueprint)
    {
        if (GameManager.Instance.money < blueprint.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        GameManager.Instance.SpendMoney(blueprint.cost);

        GameObject _tower = Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        tower = _tower;
    }

    void OnMouseEnter()
    {
        if (BuildManager.Instance.GetTowerToBuild() == null)
            return;

        rend.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    internal void UpgradeTower()
    {
        if (tower == null)
        {
            Debug.LogWarning("Không có tháp để nâng cấp!");
            return;
        }

        if (isUpgraded)
        {
            Debug.Log("Tháp đã được nâng cấp tối đa!");
            return;
        }

        TowerBlueprint blueprint = tower.GetComponent<Tower>().blueprint;

        if (GameManager.Instance.money < blueprint.upgradeCost)
        {
            Debug.Log("Không đủ tiền để nâng cấp!");
            GameManager.Instance.ShowMessage("Không đủ tiền!");
            return;
        }

        GameManager.Instance.SpendMoney(blueprint.upgradeCost);

        Vector3 position = tower.transform.position;
        Quaternion rotation = tower.transform.rotation;

        Destroy(tower);


        //// Hiệu ứng nâng cấp
        //PlayUpgradeEffect(position);

        // Âm thanh
        //AudioManager.Instance.PlaySound("Upgrade",1);

        Debug.Log("Nâng cấp tháp thành công!");
    }

    internal void SellTower()
    {
        throw new NotImplementedException();
    }
}