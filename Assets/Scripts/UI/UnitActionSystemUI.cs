using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    private List<ActionButtonUI> _actionButtonUis;

    private void Awake()
    {
        _actionButtonUis = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instace.OnSelectedUnitChanged += UnitActionSystem_onSelectedUnitChanged;
        UnitActionSystem.Instace.OnSelectedActionChanged += UnitActionSystem_OnSelctedActionChanged;
        UnitActionSystem.Instace.OnActionStarted += UnitActionSystem_OnActionStarted;
        TurnSystem.Instance.onTurnChanged += TurnSystem_OnTurnChanged;
        Unit.onAnyActionPointsChanged += Unit_OnAnyActionSpent;
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
        _actionButtonUis.Clear();
        Unit selectedUnit = UnitActionSystem.Instace.GetSelectedUnit();
        foreach (BaseAction action in selectedUnit.GetBaseActions())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);
            
            _actionButtonUis.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_onSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelctedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UpdateSelectedVisual()
    {
        foreach (var button in _actionButtonUis)
        {
            button.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instace.GetSelectedUnit();
        
        actionPointsText.text = $"Action Points: {selectedUnit.GetActionPoints()}";
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        //fixed script execution order to not get called before unit onturnchanged event
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionSpent(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
}