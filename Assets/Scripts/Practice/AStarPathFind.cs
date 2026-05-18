using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class PathNode
{
    public Vector2Int MapPosition;
    public int GCost;
    public int HCost;
    public PathNode Parent;
    public int FCost { get { return GCost + HCost; }  }

    public PathNode(Vector2Int pos)
    {
        MapPosition = pos;
    }
}

public class AStarPathFind
{
    private bool[,] _currentMap;
    public void SetCurrentMap(bool[,] map)
    {
        _currentMap = map;
    }
    Vector2Int[] _directions = new Vector2Int[]
    {
        Vector2Int.down, Vector2Int.up, Vector2Int.left,Vector2Int.right,
        new Vector2Int(1,1), new Vector2Int(-1,1),new Vector2Int(1,-1),new Vector2Int(-1,-1)
    };
    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        List<PathNode> openList = new List<PathNode>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();


        PathNode startNode = new PathNode(startPos);
        PathNode endNode = new PathNode(endPos);
        openList.Add(startNode);

        while(openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost ||
                    (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode.MapPosition);
            if(currentNode.MapPosition == endNode.MapPosition)
            {
                return RetracePath(startNode, currentNode);
            }
            foreach(var dir in _directions)
            {
                Vector2Int neighborPos = currentNode.MapPosition + dir;
                if (IsPositionValid(neighborPos) == false || closedList.Contains(neighborPos))
                {
                    continue;
                }
                int newMoveCostToNeighbor = currentNode.GCost + 1;
                PathNode neighborNode = openList.Find(n => n.MapPosition == neighborPos);
                // 람다식. LINQ. MapPosition == neighborPos인 n(노드)를 찾아서 반환
                if (neighborNode == null)
                {
                    neighborNode = new PathNode(neighborPos);
                    openList.Add(neighborNode);
                }
                neighborNode.GCost = newMoveCostToNeighbor;
                neighborNode.HCost = Mathf.Abs(neighborPos.x - endPos.x) + Mathf.Abs(neighborPos.y - endPos.y);

                neighborNode.Parent = currentNode;
            }
        }
        return null;
    }

    private List<Vector2Int> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.MapPosition);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
    private bool IsPositionValid(Vector2Int pos)
    {
        int width = _currentMap.GetLength(0);
        int height = _currentMap.GetLength(1);

        if(pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
        {
            return _currentMap[pos.x, pos.y];
        }
        return false;
    }
}
