using System.Transactions;
using UnityEngine;

public static class MyUtil
{
    public static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(0,1), // up
        new Vector2Int(1,1), // right up
        new Vector2Int(1,0), // right
        new Vector2Int(1,-1), // right down
        new Vector2Int(0,-1), // down
        new Vector2Int(-1,-1), // left down
        new Vector2Int(-1,0), // left
        new Vector2Int(-1,1)
    };
    public static (Vector2Int, Vector2Int) GetAdjacentDirections(Vector2Int direction)
    {
        int currentDirection = -1;
        for (int i = 0; i < Directions.Length; i++)
        {
            if(direction == Directions[i])
            {
                currentDirection = i;
            }
        }
        Vector2Int leftDir = Directions[(currentDirection + 7) % 8];
        Vector2Int rightDir = Directions[(currentDirection + 1) % 8];
        return (leftDir, rightDir);
    }
    public static int GetDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }
}
