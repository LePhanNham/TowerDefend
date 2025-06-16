using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public Tower tower;
    public GameObject ui; 
    public TextMeshProUGUI upgradeCostText;
    public TextMeshProUGUI sellAmountText;
    public Button upgradeButton;

    private Node target;


    public void SetTarget(Node _target)
    {
        target = _target;
        transform.position = target.GetBuildPosition();

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
        target.UpgradeTower();
        Hide();
    }

    public void Sell()
    {
        target.SellTower();
        Hide();
    }

    void UpdateUI()
    {
    }
}