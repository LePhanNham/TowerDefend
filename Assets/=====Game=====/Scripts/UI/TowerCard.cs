using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TowerCard : MonoBehaviour
{
    public static Action<TowerSettings> OnPlaceTower;
    public Image towerImage;
    public TextMeshProUGUI towerCostText;
    public TowerSettings towerloaded { get; set; }

    private DragAndDropManager dragManager;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        // button.onClick.AddListener(SetUIShop); // Tạm thời tắt để không gây lỗi nodeUI

        // Thêm DragAndDropManager
        dragManager = GetComponent<DragAndDropManager>();
        if (dragManager == null)
        {
            dragManager = gameObject.AddComponent<DragAndDropManager>();
        }

        // Thêm EventTrigger nếu chưa có
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
    }

    public void SetUpTowerBtn(TowerSettings towerSettings)
    {
        towerloaded = towerSettings;
        towerImage.sprite = towerSettings.towerIcon;
        towerCostText.text = towerSettings.cost.ToString();
        
        // Cập nhật DragAndDropManager với tower settings mới
        if (dragManager != null)
        {
            dragManager.SetTowerSettings(towerSettings);
        }
    }

    public void ToPlaceTower()
    {
        if (GameManager.Instance.CurrentMoney >= towerloaded.cost)
        {
            GameManager.Instance.SpendMoney(towerloaded.cost);
            OnPlaceTower?.Invoke(towerloaded);
        }
    }

    public void SetUIShop()
    {
        BuildManager.Instance.SelectTowerToBuild(towerloaded.towerPrefabs.GetComponent<Tower>());
    }
}
