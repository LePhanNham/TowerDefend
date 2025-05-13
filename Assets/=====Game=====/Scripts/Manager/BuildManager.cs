using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{

    private Tower towerToBuild;
    private Node selectedNode;
    [SerializeField] private List<Node> nodes;
    public override void Awake()
    {
        base.Awake();
    }

    public bool CanBuild { get { return towerToBuild != null; } }
    public bool HasMoney { get { return GameManager.Instance.money >= towerToBuild.blueprint.cost; } }

    public void SelectNode(Node node)
    {
        if (selectedNode == node)
        {
            //DeselectNode();
            return;
        }
        selectedNode = node;
        towerToBuild = null;
    }

    public void DeselectNode()
    {
        selectedNode = null;
    }


    public void SelectTowerToBuild(Tower tower)
    {
        towerToBuild = tower;
        //DeselectNode();
    }

    public Tower GetTowerToBuild()
    {
        return towerToBuild;
    }
    public void ResetTowerToBuild()
    {
        towerToBuild = null;
    }
    public void HovelNodeAvailable()
    {
        foreach (Node node in nodes)
        {
            if (node.isEmpty())
            {
                node.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                node.gameObject.GetComponent<Collider2D>().enabled = true;
                node.gameObject.GetComponent<Renderer>().material.color = node.hoverColor;

            }
        }
    }
}