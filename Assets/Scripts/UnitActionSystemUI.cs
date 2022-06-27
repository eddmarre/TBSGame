using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
        [SerializeField] private Transform actionButtonPrefab;
        [SerializeField] private Transform actionButtonContainerTransform;
        private void Start()
        {
                UnitActionSystem.Instace.OnSelectedUnitChanged += UnitActionSystem_onSelectedUnitChanged;
                CreateUnitActionButtons();
        }

        private void CreateUnitActionButtons()
        {
                foreach (Transform buttonTransform in actionButtonContainerTransform)
                {
                      Destroy(buttonTransform.gameObject);  
                }
                Unit selectedUnit =UnitActionSystem.Instace.GetSelectedUnit();
                foreach (BaseAction action in selectedUnit.GetBaseActions())
                {
                        Transform actionButtonTransform=Instantiate(actionButtonPrefab, actionButtonContainerTransform);
                        ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                        actionButtonUI.SetBaseAction(action);
                }
        }

        private void UnitActionSystem_onSelectedUnitChanged(object sender, EventArgs e)
        {
                CreateUnitActionButtons();
        }
}
