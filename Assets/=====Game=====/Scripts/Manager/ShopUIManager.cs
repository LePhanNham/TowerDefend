using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    [SerializeField] private TowerCard prefab;
    [SerializeField] private Transform panel;

    [SerializeField] public List<TowerSettings> towerSettingsList;
    public void SetUI()
    {
        for (int i = 0; i < towerSettingsList.Count; i++)
        {
            TowerCard towerCard = Instantiate(prefab);
            towerCard.transform.SetParent(panel);
            towerCard.SetUpTowerBtn(towerSettingsList[i]);
            towerCard.transform.localScale = Vector3.one;
        }
    }

    private void Awake()
    {
        SetUI();
    }
}
