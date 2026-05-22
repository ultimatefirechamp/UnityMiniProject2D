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
            // UIManager.Inst.GetCreatedUI(StatusEffectLayer).GetComponenet<StatusEffectLayer>().Regist(this)
            // Layer의 하위 오브젝트 하나 당 캐릭터 하나가 가지는 이펙트
            // 캐릭터 오브젝트 하나 당 상태이상 하나.
            // 그러면 이 statusEffect를 key로 받아서 상태이상 UI그룹을 지정.
            // 이 그룹 UI안에서도 effect를 key로 구분함. (이러기 위해서는 동일한 상태이상은 중복될 수 없음이 보장되어야 함)
            // 하나의 상태이상UI에는 스택/턴수/스프라이트를 setting할 수 있는 기능이 있음.
            // Layer[EffectComponent]->[effect]-> SetImg, SetText... 이런 애들을 Regist
            OnStatusUpdated?.Invoke(effect);
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
        
    }
}
