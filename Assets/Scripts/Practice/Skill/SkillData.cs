using UnityEngine;
using System;
using System.Collections.Generic;

public enum EffectType
{
    NONE,
    MOVE,
    DAMAGE,
    HEAL,
    KNOCKBACK
}
public enum SkillType
{
    NONE,
    ACTVIE,
    PASSIVE,
    SPECIAL
}
public abstract class Effect
{
    public EffectType Type { get; private set;  }
    protected int _value;
    public void Init(int value)
    {
        this._value = value;
    }
    public Effect(EffectType type, int value)
    {
        Type = type;
        _value = value;
    }
    public abstract void ApplyEffect(CharacterScript caster, Vector2Int target);
}

public class DamageEffect : Effect
{
    public DamageEffect(int value) : base(EffectType.DAMAGE, value) { }
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        var targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(_value);
        }
    }
}

public class HealEffect : Effect
{
    public HealEffect(int value) : base(EffectType.HEAL, value) { }
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        var targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if (targetCharacter != null)
        {
            targetCharacter.Heal(_value);
        }
    }
}

public class KnockBackEffect : Effect
{
    public KnockBackEffect(int value) : base(EffectType.KNOCKBACK, value) { }
    public override void ApplyEffect(CharacterScript caster, Vector2Int targetPos)
    {
        var target = MapManager.Inst.GetCharacterAtPosition(targetPos);
        if (target == null)
        {
            return;
        }
        int count = 0;
        Vector2Int direction = targetPos - caster.GridPosition;
        for (count = 0; count < _value; count++)
        {
            Vector2Int knockbackPos = target.GridPosition + direction;
            if (MapManager.Inst.IsWalkable(knockbackPos) == false || MapManager.Inst.IsOccupied(knockbackPos)) // can't knock back
            {
                break;
            }
            target.Move(direction);
        }
    }
}


[System.Serializable]
public class SkillData : GameDataBase // 데이터 드리븐을 통해 받을 스킬의 데이터 양식
{
    public string Name;
    public string Description;
    public int CostSP;
    public string Type;
    public int CastRange;
    public string[] EffectList;
}

public class Skill // 실질적으로 게임에 적용될 양식. 스킬이 객체적으로 생성될 때 SkillData -> Skill로 변환하면서 생성.
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int CostSP { get; private set; }
    public SkillType Type { get; private set; }
    public int CastRange { get; private set; }
    List<Effect> _effectList;
    public Skill(string id, string name, string desc, int costSp, SkillType type, int castRange, List<Effect> effectList)
    {
        Id = id;
        Name = name;
        Description = desc;
        CostSP = costSp;
        Type = type;
        CastRange = castRange;
        _effectList = effectList;
    }
    public Skill(string id, string name, string desc, int costSP)
    {
        Id = id;
        Name = name;
        Description = desc;
        CostSP = costSP;
    }
    public void SetSkillType(SkillType type)
    {
        Type = type;
    }
    public void SetCastRange(int range)
    {
        CastRange = range;
    }
    public void SetEffectList(List<Effect> effectList)
    {
        _effectList = effectList;
    }
    public void SetEffects(List<Effect> effectList)
    {
        _effectList = effectList;
    }
    public void Execute(CharacterScript caster, Vector2Int opponent)
    {
        foreach(var effect in _effectList)
        {
            effect.ApplyEffect(caster, opponent);
        }
    }
}

public static class SkillFactory
{
    public static Skill CreateSkill(SkillData data)
    {
        // 방해된다... 코드 멋대로 작성하지 말아줘 IDE야...
        // 안보이자나...

        // 스킬타입 파싱
        if(Enum.TryParse(data.Type, true, out SkillType parsedSkillData) == false)
        {
            parsedSkillData = SkillType.NONE;
        }
        // effect list 파싱
        // string데이터 양식은 DAMAGE:4 같은 EffectType:Value
        List<Effect> parsedEffect = new List<Effect>();

        foreach (var effectString  in data.EffectList)
        {
            string[] effectParam = effectString.Split(':');
            if(effectParam.Length != 2)
            {
                continue;
            }
            string effectName = effectParam[0].Trim();
            if(int.TryParse(effectParam[1].Trim(), out int effectValue) == false)
            {
                continue;
            }
            Effect effect = SkillFactory.CreateEffect(effectName, effectValue);
            if(effect == null)
            {
                continue;
            }
            parsedEffect.Add(effect);
        }

        if(parsedEffect.Count == 0)
        {
            return null;
        }
        return new Skill(data.Id, data.Name,data.Description,data.CostSP, parsedSkillData, data.CastRange, parsedEffect);
    }
    public static Effect CreateEffect(string typeString, int value)
    {
        if(Enum.TryParse(typeString, true, out EffectType type) == false)
        {
            return null;
        }
        switch(type)
        {
            case EffectType.DAMAGE:
                return new DamageEffect(value);
            case EffectType.HEAL:
                return new HealEffect(value);
            case EffectType.KNOCKBACK:
                return new KnockBackEffect(value);
        }
        return null;
    }
}