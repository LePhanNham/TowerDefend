using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationTower : GamePopup<InformationTower>
{
    [SerializeField] private GameObject popupPanel;

    //[SerializeField] private GameObject towerInfoPanel;
    //[SerializeField] private GameObject towerInfoContent;
    //[SerializeField] private GameObject towerInfoPrefab;
    [SerializeField] private Button inforTower;
    [SerializeField] private Button Exitbtn;

    protected override void Awake()
    {
        base.Awake();
        inforTower.onClick.AddListener(ShowInformationTower);
        Exitbtn.onClick.AddListener(HideInformationTower);

    }
    private void OnEnable()
    {
        SetUpUI();
    }

    public void ShowInformationTower()
    {
        popupPanel.SetActive(true);
        //if (towerInfoPanel != null && towerInfoContent != null && towerInfoPrefab != null)
        //{
        //    towerInfoPanel.SetActive(true);
        //    towerInfoContent.SetActive(true);
        //    Exitbtn.SetActive(true);
        //    // Clear previous content
        //    foreach (Transform child in towerInfoContent.transform)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //    // Add new content
        //    GameObject newTowerInfo = Instantiate(towerInfoPrefab, towerInfoContent.transform);
        //    // Set up the newTowerInfo with relevant data here
        //}
        //else
        //{
        //    Debug.LogError("Tower info panel or content is not set up correctly.");
        //}
    }
    public void HideInformationTower()
    {
        popupPanel.SetActive(false);
        //if (towerInfoPanel != null && towerInfoContent != null)
        //{
        //    towerInfoPanel.SetActive(false);
        //    towerInfoContent.SetActive(false);
        //}
    }
    public void SetUpUI()
    {
        
    }

}
