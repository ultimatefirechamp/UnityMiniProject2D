using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;

    public void SpawnEnemy(Vector2Int gridPosition)
    {
        if(MapManager.Inst.IsOccupied(gridPosition))
        {
            return; 
        }
        GameObject createdEnemyObj = Instantiate(_enemyPrefab);
        createdEnemyObj.transform.position = MapManager.Inst.ArrayToWorldPos(gridPosition.x, gridPosition.y);
        CharacterScript createdEnemy = createdEnemyObj.GetComponent<CharacterScript>();
        createdEnemy.SetGridPosition(gridPosition);
        MapManager.Inst.OccupyTile(gridPosition, createdEnemy);
    }

}
