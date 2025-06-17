using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class NodeUI : MonoBehaviour
{
    // public Tower tower; // Có thể bỏ nếu chỉ lấy thông tin từ selectedTowerInstance
    public GameObject ui; 
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI sellAmountText;
    public Button upgradeButton;

    // private Node target; // Không dùng Node nữa
    private Vector3Int currentTilePos; // Lưu tọa độ ô Tilemap được chọn
    private GameObject currentTowerInstance; // Lưu instance của tower được chọn
    private Tower currentTowerComponent; // Tham chiếu đến script Tower trên instance đó

    public void SetTarget(Vector3Int tilePos, GameObject towerInstance)
    {
        currentTilePos = tilePos;
        currentTowerInstance = towerInstance;
        currentTowerComponent = towerInstance != null ? towerInstance.GetComponent<Tower>() : null;

        // Đặt vị trí UI tại giữa ô Tilemap được chọn
        if (TileMapManager.Instance != null && TileMapManager.Instance.PlaceableTilemap != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(TileMapManager.Instance.PlaceableTilemap.GetCellCenterWorld(tilePos));
        }
        else
        {
            Debug.LogError("TileMapManager hoặc PlaceableTilemap chưa được thiết lập khi NodeUI cố định vị trí!");
            transform.position = Camera.main.WorldToScreenPoint(Vector3.zero); // Vị trí mặc định
        }
        
        UpdateUI();

        ui.SetActive(true);
    }

    public void Hide()
    {
        ui.SetActive(false);
    }

    public void Show()
    {
        ui.SetActive(true);
    }

    public void Upgrade()
    {
        if (currentTowerInstance != null)
        {
            TileMapManager.Instance.UpgradeTower(currentTilePos); // Gọi TileMapManager để nâng cấp
            // Sau khi nâng cấp, có thể cần gọi lại UpdateUI() nếu tower thay đổi hình dạng/cost
            UpdateUI();
        }
        Hide();
    }

    public void Sell()
    {
        if (currentTowerInstance != null)
        {
            TileMapManager.Instance.SellTower(currentTilePos); // Gọi TileMapManager để bán
        }
        Hide();
    }

    void UpdateUI()
    {
        if (currentTowerComponent != null)
        {
            // Cập nhật text cho chi phí nâng cấp và số tiền bán
            if (upgradeCostText != null) upgradeCostText.text = "Upgrade: " + currentTowerComponent.blueprint.upgradeCost + "$";
            if (sellAmountText != null) sellAmountText.text = "Sell: " + currentTowerComponent.blueprint.GetSellAmount(currentTowerComponent.isUpgraded) + "$";
            
            // Vô hiệu hóa nút nâng cấp nếu tower đã nâng cấp tối đa hoặc không đủ tiền
            if (upgradeButton != null)
            {
                // Logic này sẽ phức tạp hơn khi có nhiều cấp độ nâng cấp
                // Tạm thời, vô hiệu hóa nếu đã nâng cấp 1 lần hoặc không đủ tiền
                upgradeButton.interactable = !currentTowerComponent.isUpgraded && GameManager.Instance.CurrentMoney >= currentTowerComponent.blueprint.upgradeCost;
            }
        }
        else
        {
            // Nếu không có tower, ẩn thông tin hoặc hiển thị mặc định
            if (upgradeCostText != null) upgradeCostText.text = "";
            if (sellAmountText != null) sellAmountText.text = "";
            if (upgradeButton != null) upgradeButton.interactable = false;
        }
    }
}