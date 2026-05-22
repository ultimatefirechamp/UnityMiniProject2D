using UnityEngine;


public class PlayerSet : MonoBehaviour
{
    MonsterData playerData = new MonsterData();
    CharacterScript _player;
    HUDLayout hud;
    bool _isSet = false;
    private void Start()
    {
        playerData.HP = 20;
        playerData.AC = 1;
        playerData.Range = 1;
        playerData.ATK = 3;
        _isSet = true;
        if (gameObject.TryGetComponent<CharacterScript>(out _player) == false)
        {
            return;
        }
        if(hud == null)
        {
            hud = PracticeUIManager.Inst.GetCreatedUI(UIType.HUD).GetComponent<HUDLayout>();
            hud.RegistPlayer(_player);
        }
        _player.SetCharacter(playerData);
    }
    private void OnEnable()
    {
        if(PracticeUIManager.Inst == null)
        {
            return; 
        }
        hud = PracticeUIManager.Inst.GetCreatedUI(UIType.HUD).GetComponent<HUDLayout>();
        hud.RegistPlayer(_player);
    }
    private void OnDisable()
    {
        if(_player == null)
        {
            return;
        }
        hud.UnRegistPlayer(_player);
        hud = null;
        PracticeUIManager.Inst.CloseUIFromDic(UIType.HUD);
    }
}
