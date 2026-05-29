
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
        createdEnemy.SetCharacter(GameDataManager.Instance.GetMonsterData("mob_bomber"));
    }

    public void Start()
    {
        SpawnEnemy(new Vector2Int(5,3));
    }
    public void Spawn()
    {

    }
}