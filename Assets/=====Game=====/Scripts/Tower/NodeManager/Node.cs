using System;
using UnityEngine;
// using UnityEngine.EventSystems; // Không cần EventSystems nữa

public class Node : MonoBehaviour // Tên Node giờ hơi gây nhầm lẫn, nhưng sẽ giữ lại theo yêu cầu trước đó.
{
    // [SerializeField] private Color hoverColor; // Xóa: không còn là ô đất
    // [SerializeField] private Color notEnoughMoneyColor; // Xóa: không còn là ô đất
    [SerializeField] private Vector3 positionOffset; // Có thể giữ nếu cần offset vị trí tower so với tâm ô
    // [SerializeField] private LayerMask towerLayer; // Xóa: không cần cho Node trên Tower

    [Header("Optional")]
    [SerializeField] private GameObject turret; // Giữ: phần hình ảnh của tower

    private Tower tower; // Tham chiếu đến script Tower trên cùng GameObject này
    // private Renderer rend; // Xóa: không cần render ô đất
    // private Color startColor; // Xóa: không cần màu ô đất
    private bool isUpgraded = false;
    // private Vector3 originalPosition; // Xóa: không cần vị trí ban đầu của ô đất
    // private string colorPropertyName = "_Color"; // Xóa: không cần màu ô đất
    // private bool isHovered = false; // Xóa: không cần hover ô đất

    // Xóa các public property liên quan đến màu ô đất
    // public Color HoverColor => hoverColor;
    // public Color NotEnoughMoneyColor => notEnoughMoneyColor;

    public Vector3Int CurrentTilePos { get; private set; } // Vị trí ô Tilemap mà tower này đang đứng

    private void Start()
    {
        // Xóa logic liên quan đến Renderer và Collider của ô đất
        // rend = GetComponent<Renderer>();
        // if (rend != null && rend.material != null)
        // {
        //     ...
        // }
        // originalPosition = transform.position;
        // if (GetComponent<Collider2D>() == null)
        // {
        //     ...
        // }

        // Lấy component Tower trên GameObject này (prefab tower)
        tower = GetComponent<Tower>();
        if (tower == null)
        {
            Debug.LogError("Tower component not found on Node (Placed Tower)! Make sure Tower.cs is on the tower prefab.");
        }
    }

    // Xóa phương thức isEmpty vì trạng thái trống/có tower do TileMapManager quản lý
    // public bool isEmpty()
    // {
    //     return tower == null;
    // }

    // Xóa các phương thức OnTrigger vì chúng liên quan đến ô đất
    // private void OnTriggerEnter2D(Collider2D other) { ... }
    // private void OnTriggerExit2D(Collider2D other) { ... }
    // private void UpdateNodeColor(bool isHovered) { ... }

    // Phương thức này không còn cần thiết ở đây, nó sẽ được gọi bởi TileMapManager
    // public bool BuildTowerFromUI(GameObject towerPrefab) { ... }

    // Phương thức khởi tạo cho Tower đã được đặt
    public void Initialize(Vector3Int tilePos)
    {
        CurrentTilePos = tilePos;
        // Có thể thêm các logic khởi tạo khác cho Tower tại đây, ví dụ: hiệu ứng đặt tower
    }

    // Xóa GetBuildPosition vì TileMapManager sẽ tính toán vị trí instatiate
    // public Vector3 GetBuildPosition()
    // {
    //     return transform.position + positionOffset;
    // }

    internal void UpgradeTower()
    {
        if (tower == null) return;

        // Logic kiểm tra tiền và trạng thái nâng cấp sẽ do TileMapManager xử lý
        TileMapManager.Instance.UpgradeTower(CurrentTilePos);

        // Sau khi nâng cấp, có thể instance này sẽ bị hủy và một instance mới được tạo ra
        // Nếu không, hãy cập nhật trạng thái isUpgraded tại đây.
        // isUpgraded = true; // Tùy thuộc vào cách UpgradeTower trong TileMapManager hoạt động
        Debug.Log($"Yêu cầu nâng cấp tower tại ô {CurrentTilePos}");
    }

    internal void SellTower()
    {
        if (tower == null) return;

        // Gọi TileMapManager để bán tower
        TileMapManager.Instance.SellTower(CurrentTilePos);

        // Không cần làm gì thêm ở đây vì TileMapManager sẽ hủy GameObject này
        Debug.Log($"Yêu cầu bán tower tại ô {CurrentTilePos}");
    }
}
