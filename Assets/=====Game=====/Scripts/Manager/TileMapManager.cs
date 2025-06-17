using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : Singleton<TileMapManager>
{
    [field: SerializeField] public Tilemap PlaceableTilemap { get; private set; } // Thay đổi thành public property

    // Lưu trữ các tower đã được đặt trên Tilemap
    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    public override void Awake()
    {
        base.Awake();
        if (PlaceableTilemap == null)
        {
            Debug.LogError("Placeable Tilemap chưa được gán trong TileMapManager!");
        }
    }

    // Kiểm tra xem một ô Tilemap có trống để đặt tower không
    public bool IsTileEmpty(Vector3Int cellPos)
    {
        // Đầu tiên kiểm tra xem có Tile nào ở vị trí đó không (để đảm bảo là ô có thể đặt được)
        if (!PlaceableTilemap.HasTile(cellPos))
        {
            return false; // Không có Tile, không thể đặt
        }

        // Sau đó kiểm tra xem đã có tower nào được đặt ở ô này chưa
        return !placedTowers.ContainsKey(cellPos);
    }

    // Xây dựng tower trên một ô Tilemap cụ thể
    public GameObject BuildTower(Vector3Int cellPos, GameObject towerPrefab)
    {
        if (!IsTileEmpty(cellPos))
        {
            Debug.LogWarning($"Ô ({cellPos}) đã có tower hoặc không hợp lệ.");
            return null;
        }

        // Lấy vị trí trung tâm của ô trong thế giới
        Vector3 worldPos = PlaceableTilemap.GetCellCenterWorld(cellPos);
        worldPos.z = 0; // Đảm bảo z = 0

        // Tạo instance của tower prefab
        GameObject newTower = Instantiate(towerPrefab, worldPos, Quaternion.identity);
        newTower.transform.parent = PlaceableTilemap.transform; // Đặt tower làm con của tilemap để dễ quản lý

        // Gán vị trí ô cho component Node trên tower (nếu có)
        Node nodeComponent = newTower.GetComponent<Node>();
        if (nodeComponent != null)
        {
            nodeComponent.Initialize(cellPos);
        }

        // Thêm tower vào danh sách đã đặt
        placedTowers.Add(cellPos, newTower);

        Debug.Log($"Đã đặt tower {towerPrefab.name} tại ô Tilemap: {cellPos}");
        return newTower;
    }

    // Lấy tower tại một ô Tilemap cụ thể
    public GameObject GetTowerAt(Vector3Int cellPos)
    {
        if (placedTowers.TryGetValue(cellPos, out GameObject tower))
        {
            return tower;
        }
        return null;
    }

    // Bán tower tại một ô Tilemap cụ thể
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

    // Nâng cấp tower tại một ô Tilemap cụ thể (chỉ là khung, sẽ phát triển sau)
    public void UpgradeTower(Vector3Int cellPos)
    {
        // Logic nâng cấp sẽ được thêm vào đây sau này
        Debug.Log($"Nâng cấp tower tại ô Tilemap: {cellPos}");
    }
}