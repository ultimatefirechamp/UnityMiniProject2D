using UnityEngine;
using UnityEngine.Tilemaps;
public abstract class PrefabScene : MonoBehaviour
{
    public abstract void CloseScene();
}
public class IngameScene : PrefabScene
{
    [SerializeField] Tilemap wallTile;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] Transform RootTransform;
    public override void CloseScene()
    {
        HUDLayout hud;
        hud = PracticeUIManager.Inst.GetCreatedUI(UIType.HUD).GetComponent<HUDLayout>();
        hud.UnRegistPlayer(BattleManager.Inst._player);
        Destroy(this.gameObject);
    }
    private void Start()
    {
        MapManager.Inst.SetWallTileMap(wallTile);
        SpawnPlayer();
        SpawnEnemy(new Vector2Int(2, 2));
        SpawnEnemy(new Vector2Int(1, 2));
        SpawnEnemy(new Vector2Int(29, 2));
        SpawnEnemy(new Vector2Int(28, 2));
        SpawnEnemy(new Vector2Int(2, 20));
        SpawnEnemy(new Vector2Int(2, 19));
        SpawnEnemy(new Vector2Int(29, 20));
        SpawnEnemy(new Vector2Int(28, 20));
    }
    public void SpawnPlayer()
    {
        Vector2Int position = MapManager.Inst.WorldToArrayPos(new Vector3(0.5f,0.5f,0));
        var playerObject = ObjectManager.Inst.SpawnUnit(position);
        playerObject.transform.SetParent(RootTransform);
        playerObject.AddComponent<PlayerController>();
        var player = playerObject.GetComponent<CharacterScript>();

        BattleManager.Inst._player = player;

        MonsterData playerData = new MonsterData();
        playerData.HP = 20;
        playerData.AC = 1;
        playerData.Range = 1;
        playerData.ATK = 3;
        player.SetCharacter(playerData);

        HUDLayout hud;
        hud = PracticeUIManager.Inst.GetCreatedUI(UIType.HUD).GetComponent<HUDLayout>();
        hud.RegistPlayer(player);
        
    }
    public void SpawnEnemy(Vector2Int gridPosition)
    {
        if (MapManager.Inst.IsOccupied(gridPosition))
        {
            return;
        }
        GameObject createdEnemyObj = Instantiate(_enemyPrefab, RootTransform);
        createdEnemyObj.transform.position = MapManager.Inst.ArrayToWorldPos(gridPosition.x, gridPosition.y);
        CharacterScript createdEnemy = createdEnemyObj.GetComponent<CharacterScript>();
        createdEnemy.SetGridPosition(gridPosition);
        MapManager.Inst.OccupyTile(gridPosition, createdEnemy);
        createdEnemy.SetCharacter(GameDataManager.Instance.GetMonsterData("mob_bomber"));
    }
}
