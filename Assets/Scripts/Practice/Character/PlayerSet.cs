using UnityEngine;


public class PlayerSet : MonoBehaviour
{
    MonsterData playerData = new MonsterData();
    CharacterScript _player;
    bool _isSet = false;
    private void Start()
    {
        playerData.HP = 20;
        playerData.AC = 1;
        playerData.Range = 1;
        playerData.ATK = 3;
        if (gameObject.TryGetComponent<CharacterScript>(out _player) == false)
        {
            return;
        }
        _isSet = true;
    }
}
