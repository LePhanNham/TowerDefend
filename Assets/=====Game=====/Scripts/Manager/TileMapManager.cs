using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : Singleton<TileMapManager>
{
    [field: SerializeField] public Tilemap PlaceableTilemap { get; private set; } // Thay đổi thành public property

    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    public override void Awake()
    {
        base.Awake();
        if (PlaceableTilemap == null)
        {
            Debug.LogError("Placeable Tilemap chưa được gán trong TileMapManager!");
        }
    }

    public bool IsTileEmpty(Vector3Int cellPos)
    {
        if (!PlaceableTilemap.HasTile(cellPos))
        {
            return false; 
        }

        return !placedTowers.ContainsKey(cellPos);
    }

    public GameObject BuildTower(Vector3Int cellPos, GameObject towerPrefab)
    {
        if (!IsTileEmpty(cellPos))
        {
            Debug.LogWarning($"Ô ({cellPos}) đã có tower hoặc không hợp lệ.");
            return null;
        }

        Vector3 worldPos = PlaceableTilemap.GetCellCenterWorld(cellPos);
        worldPos.z = 0; // Đảm bảo z = 0

        GameObject newTower = Instantiate(towerPrefab, worldPos, Quaternion.identity);
        newTower.transform.parent = PlaceableTilemap.transform;

        Node nodeComponent = newTower.GetComponent<Node>();
        if (nodeComponent != null)
        {
            nodeComponent.Initialize(cellPos);
        }

        placedTowers.Add(cellPos, newTower);

        Debug.Log($"Đã đặt tower {towerPrefab.name} tại ô Tilemap: {cellPos}");
        return newTower;
    }

    public GameObject GetTowerAt(Vector3Int cellPos)
    {
        if (placedTowers.TryGetValue(cellPos, out GameObject tower))
        {
            return tower;
        }
        return null;
    }

    public void SellTower(Vector3Int cellPos)
    {
        if (placedTowers.TryGetValue(cellPos, out GameObject tower))
        {
            Destroy(tower);
            placedTowers.Remove(cellPos);
            Debug.Log($"Đã bán tower tại ô Tilemap: {cellPos}");
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy tower tại ô Tilemap: {cellPos} để bán.");
        }
    }

    public void UpgradeTower(Vector3Int cellPos)
    {
        Debug.Log($"Nâng cấp tower tại ô Tilemap: {cellPos}");
    }
}