using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : Singleton<ShopUIManager>
{
    [SerializeField] private TowerCard prefab;
    [SerializeField] private Transform panel;

    [SerializeField] public List<TowerSettings> towerSettingsList;
    public bool checkCreateShop = false;
    public override void Awake()
    {
        base.Awake();
        SetUI();
    }

    public void SetUI()
    {
        checkCreateShop = true;
        for (int i = 0; i < towerSettingsList.Count; i++)
        {
            TowerCard towerCard = Instantiate(prefab);
            towerCard.GetComponentInParent<Transform>().SetParent(panel);
            towerCard.SetUpTowerBtn(towerSettingsList[i]);
            towerCard.GetComponentInParent<Transform>().localScale = Vector3.one;
        }
    }


    public void SetActiveShop()
    {
        panel.gameObject.SetActive(true);
        if (!checkCreateShop)
        {
            SetUI();
        }

    }
    public void SetUnActiveShop()
    {
        panel.gameObject.SetActive(false);
    }
}
