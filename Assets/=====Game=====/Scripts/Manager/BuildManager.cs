using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{

    private TowerBlueprint towerToBuild;
    private Node selectedNode;

    public NodeUI nodeUI;

    public override void Awake()
    {
        base.Awake();
    }

    public bool CanBuild { get { return towerToBuild != null; } }
    public bool HasMoney { get { return GameManager.Instance.money >= towerToBuild.cost; } }

    public void SelectNode(Node node)
    {
        if (selectedNode == node)
        {
            DeselectNode();
            return;
        }

        selectedNode = node;
        towerToBuild = null;

        nodeUI.SetTarget(node);
    }

    public void DeselectNode()
    {
        selectedNode = null;
        nodeUI.Hide();
    }

    public void SelectTowerToBuild(TowerBlueprint tower)
    {
        towerToBuild = tower;
        DeselectNode();
    }

    public TowerBlueprint GetTowerToBuild()
    {
        return towerToBuild;
    }
}