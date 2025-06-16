using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    [Header("Build Settings")]
    [SerializeField] private Tower towerToBuild;
    [SerializeField] private Node selectedNode;
    [SerializeField] private GameObject buildEffect;
    [SerializeField] private GameObject sellEffect;

    [Header("UI References")]
    [SerializeField] private GameObject nodeUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject sellUI;

    [SerializeField] private List<Node> nodes;

    public override void Awake()
    {
        base.Awake();
    }

    public bool CanBuild()
    {
        return towerToBuild != null && GameManager.Instance.CurrentMoney >= towerToBuild.blueprint.cost;
    }

    public void SelectNode(Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }

        selectedNode = node;
        towerToBuild = null;

        nodeUI.SetActive(true);
        nodeUI.transform.position = node.transform.position;

        if (node.isEmpty())
        {
            upgradeUI.SetActive(false);
            sellUI.SetActive(false);
        }
        else
        {
            upgradeUI.SetActive(true);
            sellUI.SetActive(true);
        }
    }

    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.SetActive(false);
    }

    public void SelectTowerToBuild(Tower tower)
    {
        towerToBuild = tower;
        DeselectNode();
    }

    public Tower GetTowerToBuild()
    {
        return towerToBuild;
    }

    public void ResetTowerToBuild()
    {
        towerToBuild = null;
    }

    public void BuildTurretOn(Node node)
    {
        if (towerToBuild == null) return;

        node.BuildTower(towerToBuild);
    }

    public void HovelNodeAvailable()
    {
        foreach (Node node in nodes)
        {
            if (node.isEmpty())
            {
                node.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                node.gameObject.GetComponent<Collider2D>().enabled = true;
                node.gameObject.GetComponent<Renderer>().material.color = node.HoverColor;
            }
        }
    }

    public void UpgradeTower()
    {
        if (selectedNode != null)
        {
            selectedNode.UpgradeTower();
        }
    }

    public void SellTower()
    {
        if (selectedNode != null)
        {
            selectedNode.SellTower();
            DeselectNode();
        }
    }
}