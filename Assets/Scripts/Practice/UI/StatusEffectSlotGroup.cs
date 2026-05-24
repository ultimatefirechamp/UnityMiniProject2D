using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSlotGroup : MonoBehaviour
{
    [SerializeField] private GameObject effectSlot_Prefab;
    [SerializeField] private Transform rootTransform;
    Dictionary<string, StatusEffectSlot> _effectDictionary = new Dictionary<string, StatusEffectSlot>();

    public void AddEffect(StatusEffect effect)
    {
        if(_effectDictionary.ContainsKey(effect.Id))
        {
            _effectDictionary[effect.Id].RefreshSlot(effect);
            return;
        }
        GameObject createdSlot = Instantiate(effectSlot_Prefab,rootTransform);
        createdSlot.name = effect.Id;
        StatusEffectSlot slot = createdSlot.GetComponent<StatusEffectSlot>();
        slot.RefreshSlot(effect);
        _effectDictionary.Add(effect.Id, slot);
    }
    public void RemoveEffect(StatusEffect effect)
    {
        if(_effectDictionary.ContainsKey(effect.Id) == false)
        {
            return;
        }
        Destroy(_effectDictionary[effect.Id].gameObject);
        _effectDictionary.Remove(effect.Id);
    }
    public void RefreshEffect(StatusEffect effect)
    {
        if (_effectDictionary.TryGetValue(effect.Id, out StatusEffectSlot slot))
        {
            slot.RefreshSlot(effect);
        }
    }
    public void AddEffectDebug()
    {
        Poison effect = new Poison(3, BattleManager.Inst._player);
        AddEffect(effect);
        MoveGroup(BattleManager.Inst._player.transform);
    }
    public void RemoveDebug()
    {
        List<string> slotkeys = new List<string>();
        foreach(var slotKV in _effectDictionary)
        {
            slotkeys.Add(slotKV.Key);
        }
        foreach (var slotId in slotkeys)
        {
            Destroy(_effectDictionary[slotId].gameObject);
            _effectDictionary.Remove(slotId);
        }
    }
    void MoveGroup(Transform transform)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        this.transform.position = screenPos;
        // 나중에 target.OnMove에 += 예정.
    }

    public void RegistCharacter(CharacterScript character)
    {
        if(character.gameObject.TryGetComponent<StatusEffectComponent>(out var targetEffectComponent))
        {
            character.OnMove += MoveGroup;
            targetEffectComponent.OnStatusAdded += AddEffect;
            targetEffectComponent.OnStatusUpdated += RefreshEffect;
            targetEffectComponent.OnStatusRemoved += RemoveEffect;
            MoveGroup(character.transform);
        }
    }
    public void UnRegistCharacter(CharacterScript character)
    {
        if(character.gameObject.TryGetComponent<StatusEffectComponent>(out var targetEffectComponent))
        {
            character.OnMove -= MoveGroup;
            targetEffectComponent.OnStatusAdded -= AddEffect;
            targetEffectComponent.OnStatusUpdated -= RefreshEffect;
            targetEffectComponent.OnStatusRemoved -= RemoveEffect;
        }
    }
}
