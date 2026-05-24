using System.Collections.Generic;
using UnityEngine;

public class StatusEffectPanel : MonoBehaviour
{
    [SerializeField] GameObject statusEffectGroup_Prefab;

    Dictionary<CharacterScript, StatusEffectSlotGroup> _effectslots = new Dictionary<CharacterScript, StatusEffectSlotGroup>();

    public void RegistCharacter(CharacterScript character)
    {
        if(_effectslots.ContainsKey(character))
        {
            return;
        }
        GameObject createdSlotgroup = Instantiate(statusEffectGroup_Prefab,this.transform);
        StatusEffectSlotGroup groupUI = createdSlotgroup.GetComponent<StatusEffectSlotGroup>();
        _effectslots.Add(character, groupUI);
        groupUI.RegistCharacter(character);
    }
    public void UnRegistCharacter(CharacterScript character)
    {
        if (_effectslots.ContainsKey(character) == false)
        {
            return;
        }
        _effectslots[character].UnRegistCharacter(character);
        Destroy(_effectslots[character]);
        _effectslots.Remove(character);
    }    
}
