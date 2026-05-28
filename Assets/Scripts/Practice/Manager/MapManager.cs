using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Inst { get { return _instance; } }

    private AStarPathFind _pathfinder;
    public Tilemap wallTilemap; 
    private bool[,] _walkableMap;
    private HashSet<Vector2Int> _occupied;
    private Dictionary<Vector2Int, CharacterScript> _characterPositions;

    private Vector3Int mapOrigin;
    public int width;
    public int height;

    private void Awake()
    {
        _instance = this;
        if(wallTilemap != null)
        {
            GenerateGridData();
        }
        _pathfinder = new AStarPathFind();
        _pathfinder.SetCurrentMap(_walkableMap);
        _occupied = new HashSet<Vector2Int>();
        _characterPositions = new Dictionary<Vector2Int, CharacterScript>();
    }
    public void SetWallTileMap(Tilemap tileMap)
    {
        this.wallTilemap = tileMap;
        GenerateGridData();
        _pathfinder.SetCurrentMap(_walkableMap);
    }

    private void GenerateGridData()
    {
        BoundsInt bounds = wallTilemap.cellBounds;

        width = bounds.size.x;
        height = bounds.size.y;
        mapOrigin = bounds.position;

        _walkableMap = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 배열의 인덱스(0, 1, 2...)를 실제 타일맵 좌표(-5, -4, -3...)로 변환
                Vector3Int tilePos = new Vector3Int(mapOrigin.x + x, mapOrigin.y + y, 0);

                // 해당 위치에 벽 타일이 있는지 검사
                TileBase wallTile = wallTilemap.GetTile(tilePos);
                if (wallTile != null)
                {
                    _walkableMap[x, y] = false;
                }
                else
                {
                    _walkableMap[x, y] = true;
                }
            }
        }

        Debug.Log($"맵 변환 완료! 크기: {width} x {height}");
    }
    public bool IsWalkable(Vector2Int pos)
    {
        if(RangeCheck(pos) == false)
        {
            Debug.LogWarning($"{pos} Out of Map Range error");
            return false;
        }
        return _walkableMap[pos.x, pos.y];
    }
    public void MoveTo(Vector2Int prev, Vector2Int current, CharacterScript character)
    {
        if(RangeCheck(current) == false)
        {
            Debug.LogWarning($"{current} Out of Map Range error");
            return;
        }
        if (_occupied.Contains(current))
        {
            Debug.LogWarning($"{current} Tile already occupied error");
            return;
        }
        _occupied.Add(current);
        _occupied.Remove(prev);
        _characterPositions.Remove(prev);
        _characterPositions[current] = character;
        character.SetGridPosition(current);
    }
    public void OccupyTile(Vector2Int current, CharacterScript character)
    {
        if(RangeCheck(current) == false)
        {
            Debug.LogWarning("Out of Map Range");
            return;
        }
        if(_occupied.Contains(current))
        {
            Debug.LogWarning("Already Occupied Tile");
        }
        _characterPositions[current] = character;
        _occupied.Add(current);
    }
    bool RangeCheck(Vector2Int pos)
    {
        if(pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
        {
            return false;
        }
        return true;
    }
    public Vector2Int WorldToArrayPos(Vector3 worldPos)
    {
        Vector3Int cellPos = wallTilemap.WorldToCell(worldPos);
        int arrayX = cellPos.x - mapOrigin.x;
        int arrayY = cellPos.y - mapOrigin.y;
        return new Vector2Int(arrayX, arrayY);
    }
    public Vector3 ArrayToWorldPos(int x, int y)
    {
        Vector3Int cellPos = new Vector3Int(mapOrigin.x + x, mapOrigin.y + y, 0);
        return wallTilemap.GetCellCenterWorld(cellPos);
    }
    public List<Vector2Int> GetPathToTarget(Vector2Int start, Vector2Int end)
    {
        return _pathfinder.FindPath(start, end);
    }
    public CharacterScript GetCharacterAtPosition(Vector2Int position)
    {
        if (_characterPositions.TryGetValue(position, out var character))
        {
            return character;
        }
        return null;
    }
    public bool IsOccupied(Vector2Int position)
    {
        if (_occupied.Contains(position))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Warning!! 이 메서드는 하나의 좌표에 한 캐릭터만이 위치하는 것을 보증하지 않습니다!
    /// </summary>
    public void ForceMove(CharacterScript mover, Vector2Int position)
    {
        Vector2Int prevPosition = mover.GridPosition;
        if (_characterPositions[prevPosition] == mover)
        {
            _occupied.Remove(prevPosition);
            _characterPositions.Remove(prevPosition);
        }
        _occupied.Add(position);
        _characterPositions[position] = mover;
        mover.SetGridPosition(position);
    }
    public void Swap(Vector2Int first, Vector2Int second)
    {
        if ((IsWalkable(first) == false) || (IsWalkable(second) == false))
        {
            return;
        }
        if ((IsOccupied(first) == false) && (IsOccupied(second) == false))
        {
            return;
        }
        
        CharacterScript firstCharacter = IsOccupied(first) ? _characterPositions[first] : null;
        CharacterScript secondCharacter = IsOccupied(second) ? _characterPositions[second] : null;

        if(firstCharacter != null)
        {
            _characterPositions[second] = firstCharacter;
            _occupied.Add(second);
            firstCharacter.SetGridPosition(second);
        }
        else
        {
            _characterPositions.Remove(second);
            _occupied.Remove(second);
        }

        if (secondCharacter != null)
        {
            _characterPositions[first] = secondCharacter;
            _occupied.Add(first);
            secondCharacter.SetGridPosition(first);
        }
        else
        {
            _characterPositions.Remove(first);
            _occupied.Remove(first);
        }
    }
    public void ClearTile(Vector2Int position)
    {
        if(_occupied.Contains(position))
        {
            _occupied.Remove(position);
        }
        if(_characterPositions.ContainsKey(position))
        {
            _characterPositions.Remove(position);
        }
    }
    private void OnDrawGizmos()
    {
        if(_walkableMap == null)
        { return; }
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if (_walkableMap[x, y])
                {
                    continue;
                }
                Vector3 worldPos = ArrayToWorldPos(x, y);
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                Gizmos.DrawCube(worldPos, new Vector3(0.8f, 0.8f, 0));
            }
        }
    }
}