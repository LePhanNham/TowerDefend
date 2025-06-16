using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragTowerFromUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab;
    private SpriteRenderer previewTower;
    
    public void OnBeginDrag(PointerEventData eventData)
    {

        previewTower = towerPrefab.GetComponent<SpriteRenderer>();
        //previewTower.GetComponent<Collider2D>().enabled = false;
        SetGhostMode(previewTower, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        previewTower.transform.position = mouseWorldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Node"))
        {
            Node node = hit.collider.GetComponent<Node>();
            //if (node != null && node.isEmpty())
            //{
            //    TowerPlacementPopup.Instance.Show(node, towerPrefab);
            //}
        }

        Destroy(previewTower);
    }
    private void SetGhostMode(SpriteRenderer obj, bool isGhost)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = isGhost ? new Color(1f, 1f, 1f, 0.5f) : Color.white;
    }

}
