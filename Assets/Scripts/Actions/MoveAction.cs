using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler onStartMoving;
    public event EventHandler onStopMoving;


    [SerializeField] private int maxMovementDistance = 4;

    private List<Vector3> _targetPositions;
    private int currentPositionIndex;
    float stoppingDistance = .1f;

    public override string GetActionName()
    {
        return "Move";
    }

    private void Update()
    {
        if (!_isActive) return;
        Vector3 targetPosition = _targetPositions[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            float moveSpeed = 5f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= _targetPositions.Count)
            {
                onStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

        //change direction over time instead of instant
        float rotateSpeed = 5f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPostion = _unit.GetGridPosition();
        for (int x = -maxMovementDistance; x <= maxMovementDistance; x++)
        {
            for (int z = -maxMovementDistance; z <= maxMovementDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPostion + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPostition(testGridPosition))
                {
                    continue;
                }

                if (unitGridPostion == testGridPosition)
                {
                    //current grid position where unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //grid position already occupied by another unit
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPostion, testGridPosition))
                {
                    continue;
                }

                int pathFindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPostion, testGridPosition) >
                    maxMovementDistance * pathFindingDistanceMultiplier)
                {
                    //path length is too long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositions =
            Pathfinding.Instance.FindPath(_unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        _targetPositions = new List<Vector3>();
        foreach (GridPosition position in pathGridPositions)
        {
            _targetPositions.Add(LevelGrid.Instance.GetWorldPosition(position));
        }

        onStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition _gridPosition)
    {
        int targetCountAtPosition = _unit.GetAction<ShootAction>().GetTargetCountAtPosition(_gridPosition);
        return new EnemyAIAction
        {
            gridPosition = _gridPosition,
            actionValue = targetCountAtPosition * 10
        };
    }
}