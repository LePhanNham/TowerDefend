using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TowerPlacementPopup : MonoBehaviour
{
    public static TowerPlacementPopup Instance;

    [SerializeField] private GameObject popupPanel;
    private Vector3Int currentTilePos;
    private GameObject towerPrefab;

    private void Awake()
    {
        Instance = this;
        popupPanel.SetActive(false);
    }

    public void Show(Vector3Int tilePos, GameObject prefab)
    {
        currentTilePos = tilePos;
        towerPrefab = prefab;
        popupPanel.SetActive(true);

        if (TileMapManager.Instance != null && TileMapManager.Instance.PlaceableTilemap != null)
        {
            Vector3 worldPos = TileMapManager.Instance.PlaceableTilemap.GetCellCenterWorld(tilePos);
            popupPanel.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
        else
        {
            Debug.LogError("TileMapManager hoặc PlaceableTilemap chưa được thiết lập!");
            popupPanel.transform.position = Camera.main.WorldToScreenPoint(Vector3.zero);
        }
    }

    public void OnConfirm()
    {
        if (TileMapManager.Instance != null && towerPrefab != null)
        {
            TileMapManager.Instance.BuildTower(currentTilePos, towerPrefab);
        }
        else
        {
            Debug.LogError("Không thể xây tower: TileMapManager hoặc towerPrefab không hợp lệ.");
        }
        popupPanel.SetActive(false);
    }

    public void OnCancel()
    {
        popupPanel.SetActive(false);
    }
}
