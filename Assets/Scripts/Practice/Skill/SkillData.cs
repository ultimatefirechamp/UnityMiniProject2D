using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Runtime.InteropServices;

public enum EffectType
{
    NONE,
    MOVE,
    DAMAGE,
    HEAL,
    SELFHEAL,
    KNOCKBACK,
    DASHSLASH
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
    protected int[] _values;
    public void Init(int value)
    {
        this._value = value;
    }
    public Effect(EffectType type, int value)
    {
        Type = type;
        _value = value;
    }
    public Effect(EffectType type, int[] values)
    {
        Type = type;
        _values = values;
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
    public HealEffect(int value) : base(EffectType.SELFHEAL, value) { }
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        var targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if (targetCharacter != null)
        {
            targetCharacter.Heal(_value);
        }
    }
}
public class SelfHealEffect : Effect
{
    public SelfHealEffect(int value) : base(EffectType.HEAL, value) { }
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        caster.Heal(_value);
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
public class DashSlashEffect : Effect
{
    public DashSlashEffect(int[] values) : base(EffectType.DASHSLASH, values) { }
    
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        int moveDistance = _values[0];
        int damage = _values[1];

        Vector2Int direction = target - caster.GridPosition;

        // n칸 이동, 경로의 적들에게 데미지, 이동하려는 자리가 점유되어있다면 그 캐릭터 옮기기...
        // 1. 이동방향 * n을 한칸씩 가면서 벽인지, 적인지를 체크
        // 다음 방향 == 벽은 즉시 stop
        // 다음 방향 == 적은 계속 진행
        // 지나가는 적들을 일단 배열로 저장?
        // pos = currentpos
        // count = 0
        // for (i = 0 ~ 4)
        Stack<CharacterScript> passingEnemy = new Stack<CharacterScript>();
        Vector2Int position = caster.GridPosition;
        int count = 0;
        for(int i = 0; i < moveDistance; i++)
        {
            position = position + direction;
            if(MapManager.Inst.IsWalkable(position) == false)
            {
                position = position - direction;
                break;
            }
            count++;
            if(MapManager.Inst.IsOccupied(position))
            {
                CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position);
                enemy.TakeDamage(damage);
                passingEnemy.Push(enemy);
            }
        }
        bool isDestOccupied = MapManager.Inst.IsOccupied(position);
        
        MapManager.Inst.ForceMove(caster, position);

        if(isDestOccupied == false)
        {
            return;
        }
        for (int i = 0; i < count; i++)
        {
            position = position - direction;
            if(passingEnemy.Count == 0) { break; }
            CharacterScript enemy = passingEnemy.Pop();
            if(MapManager.Inst.IsOccupied(position))
            {
                MapManager.Inst.ForceMove(enemy, position);
            }
            else
            {
                MapManager.Inst.ForceMove(enemy, position);
                break;
            }
        }
        // {
        // pos = pos + dir
        // if (pos.wall) pos-dir; break 벽을 만나면 이전 위치로
        // count++
        // if (pos.occupied) enemyStack.enque(pos.character)
        // }
        // if(pos.occupied == false) playerMoveto(pos) <- 이 경우 enemy 옮기는 과정은 스킵
        // for(i= 0 ~ count)
        // {
        // pos = pos - dir
        // if(pos.occupied) enemyStack.deque.Forcemove(pos)
        // else enemyStack.deque.Forcemove(pos) break
        // }
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

    // 
    // 강사님 피드백 : 차라리 Data를 소유하고 있는 구조는 어떤가?
    // 스킬 레코드
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
            int[] effectValues = new int[effectParam.Length-1];
            if(effectParam.Length < 2)
            {
                continue;   
            }
            for (int i = 1; i < effectParam.Length; i++)
            {
                if (int.TryParse(effectParam[i].Trim(), out int value) == false)
                {
                    continue;
                }
                effectValues[i-1] = value;
            }
            string effectName = effectParam[0].Trim();
            //if(int.TryParse(effectParam[1].Trim(), out int effectValue) == false)
            //{
            //    continue;
            //}
            Effect effect = SkillFactory.CreateEffect(effectName, effectValues);
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
    public static Effect CreateEffect(string typeString, int[] value)
    {
        if(Enum.TryParse(typeString, true, out EffectType type) == false)
        {
            return null;
        }
        switch(type)
        {
            case EffectType.DAMAGE:
                return new DamageEffect(value[0]);
            case EffectType.HEAL:
                return new HealEffect(value[0]);
            case EffectType.SELFHEAL:
                return new SelfHealEffect(value[0]);
            case EffectType.KNOCKBACK:
                return new KnockBackEffect(value[0]);
            case EffectType.DASHSLASH:
                return new DashSlashEffect(value);
        }
        return null;
    }
}