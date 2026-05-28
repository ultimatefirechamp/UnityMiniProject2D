using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] Transform RootTransform;
    private void Start()
    {
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SpawnPlayer();
        }
    }
    public void SpawnPlayer()
    {
        Vector2Int position = MapManager.Inst.WorldToArrayPos(this.transform.position);
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
}
