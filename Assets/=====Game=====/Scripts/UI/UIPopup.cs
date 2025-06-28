using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : GamePopup<UIPopup>
{
    [SerializeField] private GameObject popupPanel;

    [SerializeField] private Button Inputbtn;
    [SerializeField] private Button Exitbtn;

    protected override void Awake()
    {
        base.Awake();
    }

    //private void onClickbtn()
    //{
    //    Inputbtn.onClick.AddListener(onClickbtnInput);
    //    Exitbtn.onClick.AddListener(onClickbtnExit);
    //}


    public void onClickbtnInput()
    {
        popupPanel.SetActive(true);
        AnimateOpen();
        Exitbtn.gameObject.SetActive(true);
    }

    public void onClickbtnExit()
    {
        Exitbtn.gameObject.SetActive(false);
        AnimateClose();
        popupPanel.SetActive(false);
    }

    protected override void OnAnimateComplete()
    {
        base.OnAnimateComplete();
    }
}
