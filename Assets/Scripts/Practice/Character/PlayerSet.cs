using UnityEngine;


public class PlayerSet : MonoBehaviour
{
    MonsterData playerData = new MonsterData();
    CharacterScript _player;
    HUDLayout hud;
    bool _isSet = false;
    private void Awake()
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
    }
    private void Start()
    {
        if(hud == null)
        {
            hud = PracticeUIManager.Inst.GetCreatedUI(UIType.HUD).GetComponent<HUDLayout>();
            hud.RegistPlayer(_player);
        }

        _player.SetCharacter(playerData);
        //StatusEffect effect = new Invincible(99, _player);
        //_player.AddStatusEffect(effect);
    }
    private void OnEnable()
    {
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