using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition _gridPosition;
    private int _gCost;
    private int _hCost;
    private int _fCost;
    private PathNode _cameFromPathNode;
    private bool _isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return _gridPosition.ToString();
    }

    public int GetGCost()
    {
        return _gCost;
    }

    public int GetFCost()
    {
        return _fCost;
    }

    public int GetHCost()
    {
        return _hCost;
    }

    public void SetGCost(int Gcost)
    {
        _gCost = Gcost;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost + _hCost;
    }

    public void SetHCost(int Hcost)
    {
        _hCost = Hcost;
    }

    public void ResetCameFromPathNode()
    {
        _cameFromPathNode = null;
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    public void SetCameFromPathNode(PathNode cameFrom)
    {
        _cameFromPathNode = cameFrom;
    }

    public PathNode GetCameFromPathNode()
    {
        return _cameFromPathNode;
    }

    public bool IsWalkable()
    {
        return _isWalkable;
    }

    public void SetWalkable(bool isWalkable)
    {
        _isWalkable = isWalkable;
    }
}