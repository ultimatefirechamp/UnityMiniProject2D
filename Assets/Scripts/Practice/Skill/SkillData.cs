using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public enum EffectType
{
    NONE,
    MOVE,
    DAMAGE,
    HEAL,
    SELFHEAL,
    KNOCKBACK,
    DASHSLASH,
    WALLRUN,
    POISON
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
            targetCharacter.TakeDamage(_value, caster);
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
        
        List<CharacterScript> passingEnemy = new List<CharacterScript>();
        Vector2Int originPosition = caster.GridPosition;
        Vector2Int checkPosition = caster.GridPosition;
        // 위치이동 전 검사 파트
        for (int i = 0; i < moveDistance; i++)
        {
            checkPosition = checkPosition + direction;
            if (MapManager.Inst.IsWalkable(checkPosition) == false)
            {
                checkPosition = checkPosition - direction;
                break;
            }
            if (MapManager.Inst.IsOccupied(checkPosition))
            {
                CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(checkPosition);
                passingEnemy.Add(enemy);
            }
        }
        // 위치 이동파트
        bool isDestOccupied;
        while (checkPosition != originPosition)
        {
            isDestOccupied = MapManager.Inst.IsOccupied(checkPosition);
            MapManager.Inst.Swap(originPosition, checkPosition);
            checkPosition = checkPosition - direction;
            if (isDestOccupied)
            {
                break;
            }
        }
        // 데미지 입히는 파트
        foreach (var enemy in passingEnemy)
        {
            if (enemy == null)
            {
                Debug.LogWarning("Enemy Is NULL");
            }
            enemy.TakeDamage(damage, caster);

        }
    }
    public void ALT_ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        int moveDistance = _values[0];
        int damage = _values[1];

        Vector2Int direction = target - caster.GridPosition;

        Stack<CharacterScript> pushGroup = new Stack<CharacterScript>();
        List<CharacterScript> passingEnemy = new List<CharacterScript>();
        Vector2Int position = caster.GridPosition;
        int count = 0;
        // 위치이동 전 검사 파트
        for (int i = 0; i < moveDistance; i++)
        {
            position = position + direction;
            if (MapManager.Inst.IsWalkable(position) == false)
            {
                position = position - direction;
                break;
            }
            count++;
            if (MapManager.Inst.IsOccupied(position))
            {
                CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position);
                pushGroup.Push(enemy);
                passingEnemy.Add(enemy);
            }
        }
        // 위치 이동파트
        bool isDestOccupied = MapManager.Inst.IsOccupied(position);
        MapManager.Inst.ForceMove(caster, position);
        for (int i = 0; i < count; i++)
        {
            if(isDestOccupied == false)
            {
                break; 
            }
            position = position - direction;
            if (pushGroup.Count == 0) { break; }
            CharacterScript enemy = pushGroup.Pop();
            if (MapManager.Inst.IsOccupied(position))
            {
                MapManager.Inst.ForceMove(enemy, position);
            }
            else
            {
                MapManager.Inst.ForceMove(enemy, position);
                break;
            }
        }

        // 데미지 입히는 파트
        foreach(var enemy in passingEnemy)
        {
            if(enemy != null)
            {
                enemy.TakeDamage(damage, caster);
            }
        }
    }
}
public class PoisonEffect : Effect
{
    public PoisonEffect(int values) : base(EffectType.POISON, values){ }
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        
    }
}

// 현재 InstantKill이후의 로직 구현안되어있으므로 절대 사용금지!!
public class WallRunEffect : Effect
{
    public WallRunEffect(int[] values) : base(EffectType.WALLRUN, values) { }
    public override void ApplyEffect(CharacterScript caster, Vector2Int target)
    {
        int moveDistance = _values[0];
        int damage = _values[1];

        Vector2Int oppositeDirection = caster.GridPosition - target;
        // 입력한 방향과 역방향으로 distance만큼으로 이동
        // 벽을 만나면 멈춤 
        // 이동한 칸에 있던 적은 즉사
        // 이동한 칸과 인접한 모든 적에게 데미지
        // 
        Vector2Int position = caster.GridPosition;
        for (int i = 0; i < moveDistance; i++) 
        {
            position = position + oppositeDirection;
            if(MapManager.Inst.IsWalkable(position) == false)
            {
                position = position - oppositeDirection;
                break;
            }
        }
        if(MapManager.Inst.IsOccupied(position) && position == caster.GridPosition)
        {
            CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position);
            enemy.InstantKill();
        }
        MapManager.Inst.ForceMove(caster, position);
        foreach(var direction in MyUtil.Directions)
        {
            CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position + direction);
            if (enemy != null)
            {
                enemy.TakeDamage(damage, caster);
            }
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