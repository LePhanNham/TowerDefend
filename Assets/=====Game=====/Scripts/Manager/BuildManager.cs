using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildManager : Singleton<BuildManager>
{
    [Header("Build Settings")]
    [SerializeField] private Tower towerToBuild;
    private Vector3Int selectedTilePos;
    private GameObject selectedTowerInstance;

    [SerializeField] private GameObject buildEffect;
    [SerializeField] private GameObject sellEffect;

    [Header("UI References")]
    [SerializeField] private GameObject nodeUI;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private GameObject sellUI;

    public override void Awake()
    {
        base.Awake();
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (nodeUI == null)
        {
            Debug.LogWarning("nodeUI chưa được gán trong BuildManager!");
        }
        if (upgradeUI == null)
        {
            Debug.LogWarning("upgradeUI chưa được gán trong BuildManager!");
        }
        if (sellUI == null)
        {
            Debug.LogWarning("sellUI chưa được gán trong BuildManager!");
        }
    }

    public bool CanBuild()
    {
        return towerToBuild != null && GameManager.Instance.CurrentMoney >= towerToBuild.blueprint.cost;
    }

    public void SelectTile(Vector3Int tilePos)
    {
        if (selectedTilePos == tilePos)
        {
            DeselectTile();
            return;
        }

        selectedTilePos = tilePos;
        towerToBuild = null;

        selectedTowerInstance = TileMapManager.Instance.GetTowerAt(tilePos);

        if (nodeUI != null)
        {
            nodeUI.SetActive(true);
            nodeUI.transform.position = TileMapManager.Instance.PlaceableTilemap.GetCellCenterWorld(tilePos);
        }

        if (selectedTowerInstance != null)
        {
            if (upgradeUI != null) upgradeUI.SetActive(true);
            if (sellUI != null) sellUI.SetActive(true);
        }
        else
        {
            if (upgradeUI != null) upgradeUI.SetActive(false);
            if (sellUI != null) sellUI.SetActive(false);
        }
    }

    public void DeselectTile()
    {
        selectedTilePos = Vector3Int.zero;
        selectedTowerInstance = null;
        if (nodeUI != null)
        {
            nodeUI.SetActive(false);
        }
    }

    public void SelectTowerToBuild(Tower tower)
    {
        towerToBuild = tower;
        DeselectTile();
    }

    public Tower GetTowerToBuild()
    {
        return towerToBuild;
    }

    public void ResetTowerToBuild()
    {
        towerToBuild = null;
    }

    public void UpgradeTower()
    {
        if (selectedTowerInstance != null)
        {
            TileMapManager.Instance.UpgradeTower(selectedTilePos);
        }
    }

    public void SellTower()
    {
        if (selectedTowerInstance != null)
        {
            TileMapManager.Instance.SellTower(selectedTilePos);
            DeselectTile();
        }
    }
}