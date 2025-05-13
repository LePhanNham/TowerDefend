using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Vector3 positionOffset;

    private Tower tower;
    private Renderer rend;
    private Color startColor;
    private bool isUpgraded = false;



    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    public bool isEmpty()
    {
        return tower = null;
    }
    void OnMouseDown()
    {
        if (BuildManager.Instance.GetTowerToBuild() == null)
        {
            return;
            //BuildManager.Instance.SelectNode(this);
        }

        if (tower != null)
        {
            Debug.Log("Can't build there! - TODO: Display on screen.");
            return;
        }
        BuildTower(BuildManager.Instance.GetTowerToBuild());
    }

    void BuildTower(Tower tower)
    {
        if (GameManager.Instance.money < tower.blueprint.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        GameManager.Instance.SpendMoney(tower.blueprint.cost);
        this.tower = Instantiate(tower, GetBuildPosition(), Quaternion.identity);
        this.tower.transform.SetParent(this.transform);
        tower.transform.localScale = new Vector3(0.5f,0.5f,1);
        BuildManager.Instance.ResetTowerToBuild();
    }

    void OnMouseEnter()
    {
        if (BuildManager.Instance.GetTowerToBuild() == null)
            return;

        rend.material.color = hoverColor;
        BuildManager.Instance.HovelNodeAvailable();
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


        Debug.Log("Nâng cấp tháp thành công!");
    }

    internal void SellTower()
    {
        Destroy(tower);
        GameManager.Instance.AddMoney(tower.blueprint.GetSellAmount(false));
    }
}