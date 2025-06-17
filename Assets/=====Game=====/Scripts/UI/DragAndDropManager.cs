using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DragAndDropManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tower Settings")]
    [SerializeField] private TowerSettings towerSettings;
    [SerializeField] private LayerMask validPlacementLayer;
    
    [Header("Preview Settings")]
    [SerializeField] private Color validPlacementColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color invalidPlacementColor = new Color(1, 0, 0, 0.5f);
    [SerializeField] private float previewAlpha = 0.5f;
    [SerializeField] private Material previewMaterial; // Material đặc biệt cho preview

    private GameObject previewTower;
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 originalPosition;
    private Vector3Int targetTilePos;
    private int previewLayer;

    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;

        // Tìm layer cho preview
        previewLayer = LayerMask.NameToLayer("TowerPreview");
        if (previewLayer == -1)
        {
            Debug.LogWarning("Layer 'TowerPreview' không tồn tại. Sử dụng layer mặc định.");
            previewLayer = 0; // Sử dụng layer mặc định
        }
    }

    public void SetTowerSettings(TowerSettings settings)
    {
        towerSettings = settings;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (towerSettings == null || TileMapManager.Instance == null || TileMapManager.Instance.PlaceableTilemap == null) return;

        isDragging = true;
        
        // Tạo preview tower
        if (previewTower == null)
        {
            previewTower = Instantiate(towerSettings.towerPrefabs);
            
            // Đặt layer cho preview một cách an toàn
            try
            {
                previewTower.layer = previewLayer;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Không thể đặt layer cho preview: {e.Message}");
            }

            // Vô hiệu hóa các component liên quan đến việc bắn
            var towerComponent = previewTower.GetComponent<Tower>();
            if (towerComponent != null)
            {
                towerComponent.enabled = false;
            }

            var towerProjectile = previewTower.GetComponent<TowerProjectile>();
            if (towerProjectile != null)
            {
                towerProjectile.enabled = false;
            }

            // Vô hiệu hóa collider để không ảnh hưởng đến gameplay
            var collider = previewTower.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            // Làm mờ preview
            SetPreviewTransparency(previewTower, previewAlpha);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || previewTower == null || TileMapManager.Instance == null || TileMapManager.Instance.PlaceableTilemap == null) return;

        // Lấy vị trí chuột trong thế giới và chuyển thành tọa độ ô Tilemap
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // Đảm bảo z = 0
        Vector3Int currentCell = TileMapManager.Instance.PlaceableTilemap.WorldToCell(mouseWorldPos);

        // Điều chỉnh vị trí preview về giữa ô Tilemap
        Vector3 cellCenterWorld = TileMapManager.Instance.PlaceableTilemap.GetCellCenterWorld(currentCell);
        previewTower.transform.position = cellCenterWorld;

        // Kiểm tra xem ô Tilemap hiện tại có hợp lệ để đặt tower không
        bool isValidTile = TileMapManager.Instance.IsTileEmpty(currentCell);
        
        targetTilePos = currentCell;
        SetPreviewColor(isValidTile ? validPlacementColor : invalidPlacementColor);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging || TileMapManager.Instance == null || TileMapManager.Instance.PlaceableTilemap == null) return;

        // Sử dụng targetTilePos để đặt tower
        if (CanPlaceTower())
        {
            // Đặt tower vào ô Tilemap
            PlaceTower(targetTilePos);
        }

        // Xóa preview
        if (previewTower != null)
        {
            Destroy(previewTower);
            previewTower = null;
        }

        isDragging = false;
        targetTilePos = Vector3Int.zero; // Reset về 0
    }

    private bool CanPlaceTower()
    {
        if (TileMapManager.Instance == null || TileMapManager.Instance.PlaceableTilemap == null) return false;

        // Kiểm tra xem ô có trống không bằng TileMapManager
        if (!TileMapManager.Instance.IsTileEmpty(targetTilePos)) return false;

        // Kiểm tra tiền
        return GameManager.Instance.CurrentMoney >= towerSettings.cost;
    }

    private void PlaceTower(Vector3Int cellPos)
    {
        if (towerSettings == null || TileMapManager.Instance == null || TileMapManager.Instance.PlaceableTilemap == null) return;

        // Kiểm tra tiền
        if (GameManager.Instance.CurrentMoney < towerSettings.cost)
        {
            GameManager.Instance.ShowMessage("Không đủ tiền!");
            return;
        }

        // Trừ tiền
        GameManager.Instance.SpendMoney(towerSettings.cost);

        // Gọi TileMapManager để thực hiện đặt tower
        TileMapManager.Instance.BuildTower(cellPos, towerSettings.towerPrefabs);
    }

    private void SetPreviewTransparency(GameObject preview, float alpha)
    {
        // Làm mờ tất cả renderer trong preview
        foreach (Renderer renderer in preview.GetComponentsInChildren<Renderer>())
        {
            if (previewMaterial != null)
            {
                // Sử dụng material đặc biệt cho preview
                Material[] materials = new Material[renderer.materials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = previewMaterial;
                }
                renderer.materials = materials;
            }
            else
            {
                // Fallback: sử dụng material hiện tại với độ trong suốt
                foreach (Material material in renderer.materials)
                {
                    Color color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
            }
        }
    }

    private void SetPreviewColor(Color color)
    {
        if (previewTower == null) return;

        // Đặt màu cho tất cả renderer trong preview
        foreach (Renderer renderer in previewTower.GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.color = color;
            }
        }
    }
} 