using UnityEngine;
using UnityEngine.UI;

public class TowerPlacementPopup : MonoBehaviour
{
    public static TowerPlacementPopup Instance;

    [SerializeField] private GameObject popupPanel;
    private Node currentNode;
    private GameObject towerPrefab;

    private void Awake()
    {
        Instance = this;
        popupPanel.SetActive(false);
    }

    public void Show(Node node, GameObject prefab)
    {
        currentNode = node;
        towerPrefab = prefab;
        popupPanel.SetActive(true);
        popupPanel.transform.position = Camera.main.WorldToScreenPoint(node.transform.position);
    }

    public void OnConfirm()
    {
        currentNode.BuildTowerFromUI(towerPrefab);
        popupPanel.SetActive(false);
    }

    public void OnCancel()
    {
        popupPanel.SetActive(false);
    }
}
