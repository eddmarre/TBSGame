using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> onAnyShoot;
    public event EventHandler<OnShootEventArgs> onShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }

    [SerializeField] private LayerMask obstaclesLayerMask;
    private State _state;
    private int maxShootDistance = 7;
    private float stateTimer;
    private bool _canShootBullet;
    private Unit _targetUnit;


    private void Update()
    {
        if (!_isActive) return;
        stateTimer -= Time.deltaTime;
        switch (_state)
        {
            case State.Aiming:
                Vector3 moveDirection = (_targetUnit.GetWorldPosition() - transform.position).normalized;
                float rotateSpeed = 5f;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (_canShootBullet)
                {
                    Shoot();
                    _canShootBullet = false;
                }

                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer < 0f)
        {
            NextState();
        }
    }

    private void Shoot()
    {
        onAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootingUnit = _unit
        });
        
        onShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootingUnit = _unit
        });
        _targetUnit.Damage(40);
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.Aiming:
                _state = State.Shooting;
                var shootingStateTime = .1f;
                stateTimer = shootingStateTime;
                break;

            case State.Shooting:
                _state = State.Cooloff;
                var coolOffStateTime = .5f;
                stateTimer = coolOffStateTime;
                break;

            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _canShootBullet = true;

        _targetUnit = LevelGrid.Instance.GetUnitAtAGridPosition(gridPosition);

        _state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPostion = _unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPostion);
    }


    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPostion)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPostion + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPostition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //grid position is empty no unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtAGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == _unit.IsEnemy())
                {
                    //both units on same team
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPostion);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShouldHeight = 1.7f;
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShouldHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask))
                {
                    //blocked by obstacle
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public Unit GetTargetUnit()
    {
        return _targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition _gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtAGridPosition(_gridPosition);

        return new EnemyAIAction
        {
            gridPosition = _gridPosition,
            //shoot player with the least amount of health
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f)
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}