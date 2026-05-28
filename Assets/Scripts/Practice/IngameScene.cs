using UnityEngine;
using UnityEngine.Tilemaps;

public class IngameScene : MonoBehaviour
{
    [SerializeField] Tilemap wallTile;
    private void Start()
    {
        MapManager.Inst.SetWallTileMap(wallTile);
    }
}
