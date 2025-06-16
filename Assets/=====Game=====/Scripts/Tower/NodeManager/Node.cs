using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color notEnoughMoneyColor;
    [SerializeField] private Vector3 positionOffset;

    [Header("Optional")]
    [SerializeField] private GameObject turret;

    private Tower tower;
    private Renderer rend;
    private Color startColor;
    private bool isUpgraded = false;
    private Vector3 originalPosition;

    public Color HoverColor => hoverColor;
    public Color NotEnoughMoneyColor => notEnoughMoneyColor;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        originalPosition = transform.position;
    }

    public bool isEmpty()
    {
        return tower == null;
    }

    private void OnMouseDown()
    {
        if (!BuildManager.Instance.CanBuild())
            return;

        if (turret != null)
        {
            Debug.Log("Can't build there! - TODO: Display on screen.");
            return;
        }

        if (tower != null)
        {
            Debug.Log("Can't build there!");
            return;
        }

        BuildManager.Instance.BuildTurretOn(this);
    }

    public void BuildTower(Tower tower)
    {
        if (GameManager.Instance.CurrentMoney < tower.blueprint.cost)
        {
            Debug.Log("Not enough money to build that!");
            GameManager.Instance.ShowMessage("Không đủ tiền!");
            return;
        }

        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        GameManager.Instance.SpendMoney(tower.blueprint.cost);

        this.tower = Instantiate(tower, GetBuildPosition(), Quaternion.identity);
        this.tower.transform.SetParent(this.transform);
        this.tower.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        BuildManager.Instance.ResetTowerToBuild();
    }

    public bool BuildTowerFromUI(GameObject towerPrefab)
    {
        if (!isEmpty()) return false;

        Tower towerScript = towerPrefab.GetComponent<Tower>();
        if (towerScript == null)
        {
            Debug.LogError("Prefab không có script Tower!");
            return false;
        }

        if (GameManager.Instance.CurrentMoney < towerScript.blueprint.cost)
        {
            Debug.Log("Không đủ tiền để xây!");
            GameManager.Instance.ShowMessage("Không đủ tiền!");
            return false;
        }

        GameManager.Instance.SpendMoney(towerScript.blueprint.cost);

        this.tower = Instantiate(towerPrefab, GetBuildPosition(), Quaternion.identity).GetComponent<Tower>();
        this.tower.transform.SetParent(this.transform);
        this.tower.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        return true;
    }

    private void OnMouseEnter()
    {
        if (!BuildManager.Instance.CanBuild())
            return;

        if (BuildManager.Instance.GetTowerToBuild() != null)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }

        if (BuildManager.Instance.GetTowerToBuild() == null)
            return;

        BuildManager.Instance.HovelNodeAvailable();
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    internal void UpgradeTower()
    {
        if (tower == null)
        {
            Debug.LogWarning("Không có tháp để nâng cấp!");
            return;
        }

        if (isUpgraded)
        {
            Debug.Log("Tháp đã được nâng cấp tối đa!");
            return;
        }

        TowerBlueprint blueprint = tower.GetComponent<Tower>().blueprint;

        if (GameManager.Instance.CurrentMoney < blueprint.upgradeCost)
        {
            Debug.Log("Không đủ tiền để nâng cấp!");
            GameManager.Instance.ShowMessage("Không đủ tiền!");
            return;
        }

        GameManager.Instance.SpendMoney(blueprint.upgradeCost);

        Vector3 position = tower.transform.position;
        Quaternion rotation = tower.transform.rotation;

        // TODO: Thay bằng tháp nâng cấp
        // Destroy(tower.gameObject);
        // tower = Instantiate(upgradedTowerPrefab, position, rotation).GetComponent<Tower>();

        isUpgraded = true;
        Debug.Log("Nâng cấp tháp thành công!");
    }

    internal void SellTower()
    {
        if (tower == null) return;

        GameManager.Instance.AddMoney(tower.blueprint.GetSellAmount(isUpgraded));
        Destroy(tower.gameObject);
        tower = null;

        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Lưu vị trí ban đầu của node
        originalPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Di chuyển node theo con trỏ chuột
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Kiểm tra xem node có được thả vào vị trí hợp lệ không
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("Node"))
        {
            // Nếu thả vào một node khác, thực hiện logic cần thiết
            Node targetNode = hit.collider.GetComponent<Node>();
            if (targetNode != null && targetNode.isEmpty())
            {
                // Đặt tháp vào node mới
                if (tower != null)
                {
                    tower.transform.SetParent(targetNode.transform);
                    tower.transform.position = targetNode.GetBuildPosition();
                    targetNode.tower = tower;
                    tower = null;
                }
            }
        }
        else
        {
            // Nếu không thả vào vị trí hợp lệ, trả node về vị trí ban đầu
            transform.position = originalPosition;
        }
    }
}
