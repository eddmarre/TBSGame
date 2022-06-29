using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instace.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UpdateVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (_unit == UnitActionSystem.Instace.GetSelectedUnit())
            _meshRenderer.enabled = true;
        else
            _meshRenderer.enabled = false;
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instace.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }
}