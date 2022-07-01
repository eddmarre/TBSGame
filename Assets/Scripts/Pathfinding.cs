using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }


    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGNOL_COST = 14;
    [SerializeField] private Transform gridDebugObjectPrefab;
    private int _width;
    private int _height;
    private float _cellSize;
    private GridSystem<PathNode> _gridSystem;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("there is more than one Pathfinding!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetUp(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            ((GridSystem<PathNode> system, GridPosition gridPosition) =>
                new PathNode(gridPosition)));

        GridDebugObject gridDebugObject = gridDebugObjectPrefab.GetComponent<GridDebugObject>();
        //_gridSystem.CreateDebugOBjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float rayCastOffSetDistance = 5f;

                if (Physics.Raycast(
                        worldPosition + Vector3.down * rayCastOffSetDistance,
                        Vector3.up,
                        rayCastOffSetDistance * 2,
                        obstaclesLayerMask)
                   )
                {
                    GetNode(x, z).SetWalkable(false);
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = _gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = _gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < _gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < _gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = _gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                //reached final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighborNode))
                {
                    continue;
                }

                if (!neighborNode.IsWalkable())
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() +
                                     CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                if (tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetCameFromPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //no path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int distance = Mathf.Abs(gridPositionDistance._x) + Mathf.Abs(gridPositionDistance._z);
        int xDistance = Mathf.Abs(gridPositionDistance._x);
        int zDistaince = Mathf.Abs(gridPositionDistance._z);
        int remaining = Mathf.Abs(xDistance - zDistaince);
        return MOVE_DIAGNOL_COST * Mathf.Min(xDistance, zDistaince) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostPathNode = pathNodes[0];
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodes[i];
            }
        }

        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return _gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();

        pathNodeList.Add(endNode);

        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositions = new List<GridPosition>();
        foreach (PathNode node in pathNodeList)
        {
            gridPositions.Add(node.GetGridPosition());
        }

        return gridPositions;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighborlist = new List<PathNode>();
        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition._x - 1 >= 0)
        {
            //left
            neighborlist.Add(GetNode(gridPosition._x - 1, gridPosition._z + 0));
            if (gridPosition._z - 1 >= 0)
            {
                //left down
                neighborlist.Add(GetNode(gridPosition._x - 1, gridPosition._z - 1));
            }

            if (gridPosition._z + 1 < _gridSystem.GetHeight())
            {
                //left up
                neighborlist.Add(GetNode(gridPosition._x - 1, gridPosition._z + 1));
            }
        }

        if (gridPosition._x + 1 < _gridSystem.GetWidth())
        {
            //right
            neighborlist.Add(GetNode(gridPosition._x + 1, gridPosition._z + 0));
            if (gridPosition._z - 1 >= 0)
            {
                //right down
                neighborlist.Add(GetNode(gridPosition._x + 1, gridPosition._z - 1));
            }

            if (gridPosition._z + 1 < _gridSystem.GetHeight())
            {
                //right up
                neighborlist.Add(GetNode(gridPosition._x + 1, gridPosition._z + 1));
            }
        }

        if (gridPosition._z - 1 >= 0)
        {
            //down
            neighborlist.Add(GetNode(gridPosition._x + 0, gridPosition._z - 1));
        }


        if (gridPosition._z + 1 < _gridSystem.GetHeight())
        {
            //up
            neighborlist.Add(GetNode(gridPosition._x + 0, gridPosition._z + 1));
        }


        return neighborlist;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPos, GridPosition endGridPos)
    {
        FindPath(startGridPos, endGridPos, out int pathLength);
        return pathLength;
    }
}