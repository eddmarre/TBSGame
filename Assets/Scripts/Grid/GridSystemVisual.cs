using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    public static GridSystemVisual Instace { get; private set; }


    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instace != null)
        {
            Debug.LogError("there is more than one GridSystemVisual!");
            Destroy(gameObject);
            return;
        }

        Instace = this;
    }

    private void Start()
    {
        _gridSystemVisualSingleArray =
            new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform=Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition),
                    Quaternion.identity);

                _gridSystemVisualSingleArray[x, z] =
                    gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    private void Update()
    {
        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                _gridSystemVisualSingleArray[x,z].Hide();
            }
        }

    }

    public void ShowGridPositionList(List<GridPosition> gridPositions)
    {
        foreach (var position in gridPositions)
        {
            _gridSystemVisualSingleArray[position._x,position._z].Show();
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();
        Unit selectedUnit = UnitActionSystem.Instace.GetSelectedUnit();
        ShowGridPositionList(selectedUnit.GetMoveAction().GetValidActionGridPositionList());
    }
}