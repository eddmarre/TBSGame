using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instace.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        
        Hide();
    }

    private void UnitActionSystem_OnBusyChanged(object sender,bool isBusy)
    {
        if(isBusy)
            Show();
        else
            Hide();
    }   

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
