using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class StatusEffectComponent : MonoBehaviour
{
    CharacterScript _owner;
    Dictionary<string, StatusEffect> _effects;
    public event Action<StatusEffect> OnStatusAdded;
    public event Action<StatusEffect> OnStatusUpdated;
    public event Action<StatusEffect> OnStatusRemoved;

    private void Awake()
    {
        _owner = GetComponent<CharacterScript>();
        _effects = new Dictionary<string, StatusEffect>();
    }
    private void OnEnable()
    {
        _owner.OnAddEffect += AddStatusEffect;
        _owner.OnTickStart += ProcessTurnTick;
        _owner.OnCharacterDestroy += this.OnCharacterDestroy;
    }
    private void Start()
    {
        if(_owner == null)
        {
            return;
        }
        GameObject effectUI = PracticeUIManager.Inst.GetCreatedUI(UIType.StatusEffectLayer);
        effectUI.GetComponent<StatusEffectPanel>().RegistCharacter(_owner);
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if(_effects.ContainsKey(effect.Id))
        {
            _effects[effect.Id].AddStack(effect.Duration);
            OnStatusUpdated?.Invoke(_effects[effect.Id]);
        }
        else
        {
            _effects.Add(effect.Id, effect);
            effect.OnApply();
            //OnStatusUpdated?.Invoke(_effects[effect.Id]);
            OnStatusAdded?.Invoke(_effects[effect.Id]);
        }
    }
    public void ProcessTurnTick()
    {
        List<string> removeList = new List<string>();
        foreach(var effect in _effects.Values)
        {
            effect.OnTurnTick();
            OnStatusUpdated.Invoke(effect);
            if(effect.Stack <= 0 || effect.Duration <= 0)
            {
                removeList.Add(effect.Id);
            }
        }
        foreach(var removeKey in removeList)
        {
            StatusEffect effect = _effects[removeKey];
            effect.OnRemove();
            _effects.Remove(removeKey);
            OnStatusRemoved?.Invoke(effect);
        }
    }
    public int GetModifiedDamage(int originalDamage, CharacterScript attacker = null)
    {
        int modifiedDamage = originalDamage;
        foreach (var effect in _effects.Values)
        {
            modifiedDamage = effect.ModifyDamage(originalDamage, attacker);
        }
        return modifiedDamage;
    }
    private void OnDisable()
    {
        if(_owner != null)
        {
            _owner.OnAddEffect -= AddStatusEffect;
            _owner.OnTickStart -= ProcessTurnTick;
            _owner.OnCharacterDestroy -= this.OnCharacterDestroy;
        }
    }
    void OnCharacterDestroy()
    {
        RemoveStatus();
    }
    void RemoveStatus()
    {
        GameObject effectUI = PracticeUIManager.Inst.GetCreatedUI(UIType.StatusEffectLayer);
        effectUI.GetComponent<StatusEffectPanel>().UnRegistCharacter(_owner);
    }
    private void OnDestroy()
    {
        RemoveStatus();
    }
}
