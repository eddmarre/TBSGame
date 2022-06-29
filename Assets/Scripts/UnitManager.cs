using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//raised execution order to run before any other script
public class UnitManager : MonoBehaviour
{
    private List<Unit> _unitLists;
    private List<Unit> _friendlyUnitList;
    private List<Unit> _enemyUnitList;
    public static UnitManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("there is more than one UnitManager!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _unitLists = new List<Unit>();
        _friendlyUnitList = new List<Unit>();
        _enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.onAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.onAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        _unitLists.Add(unit);
        if (unit.IsEnemy())
        {
            _enemyUnitList.Add(unit);
        }
        else
        {
            _friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        _unitLists.Remove(unit);
        if (unit.IsEnemy())
        {
            _enemyUnitList.Remove(unit);
        }
        else
        {
            _friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList()
    {
        return _unitLists;
    }
    public List<Unit> GetEnemyUnitList()
    {
        return _enemyUnitList;
    }
    public List<Unit> GetFriendlyUnitList()
    {
        return _friendlyUnitList;
    }
}