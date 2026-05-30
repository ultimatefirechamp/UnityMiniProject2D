using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.Rendering.DebugUI;

public enum EffectType
{
    NONE,
    MOVE,
    DAMAGE,
    HEAL,
    SELFHEAL,
    KNOCKBACK,
    DASHSLASH,
    WALLJUMP,
    GUILLOTINE,
    POISONOUS
}

public enum SkillComboType
{
    NONE,
    SHIFT,
    CTRL
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


public class DamageEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        int damage = payload.Values[0];
        var targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if (targetCharacter != null)
        {
            targetCharacter.TakeDamage(damage, caster);
        }
    }
}
public class HealEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        int healAmount = payload.Values[0];
        var targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if (targetCharacter != null)
        {
            targetCharacter.Heal(healAmount);
        }
    }
}
public class SelfHealEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        int healAmount = payload.Values[0];
        caster.Heal(healAmount);
    }
}
public class KnockBackEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int targetPos)
    {
        int knockDistance = payload.Values[0];
        var target = MapManager.Inst.GetCharacterAtPosition(targetPos);
        if (target == null)
        {
            return;
        }
        int count = 0;
        Vector2Int direction = targetPos - caster.GridPosition;
        for (count = 0; count < knockDistance; count++)
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
public class DashSlashEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        int moveDistance = payload.Values[0];
        int damage = payload.Values[1];

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
            if (isDestOccupied == false)
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
}
public class PoisonousEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        int stack = payload.Values[0];
        int duration = payload.Values[1];
        CharacterScript targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if(targetCharacter == null) { return; }
        StatusEffect effect = new Poison(duration, targetCharacter);
        targetCharacter.AddStatusEffect(effect);
    }
}
public class WallJumpEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        int moveDistance = payload.Values[0];
        //int damage = payload.Values[1];
        int damage = 4;

        Vector2Int oppositeDirection = caster.GridPosition - target;
        // 입력한 방향과 역방향으로 distance만큼으로 이동
        // 벽을 만나면 멈춤 
        // 이동한 칸에 있던 적은 즉사... 에서 기획 수정? 그냥 위치 바꾸기..? 
        // 이동한 칸과 인접한 모든 적에게 데미지
        // 
        Vector2Int position = caster.GridPosition;
        for (int i = 0; i < moveDistance; i++)
        {
            position = position + oppositeDirection;
            if (MapManager.Inst.IsWalkable(position) == false)
            {
                position = position - oppositeDirection;
                break;
            }
        }
        if (MapManager.Inst.IsOccupied(position) && position == caster.GridPosition) // 목표한 곳이 내가 있는 곳이라면. 즉 제자리.
        {
            return; // 효과 없음. 아마 이 조건을 만족하려면. 양 옆으로 꽉 막힌 곳에서 쓰면 이렇게 되지 않을까.
            //CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position);
            //enemy.InstantKill(caster);
        }

        if(MapManager.Inst.IsOccupied(position) && position != caster.GridPosition) // 왜 비슷한 조건문 검사를 여러번 하나요?
            // 사실 위에 조건문을 처음에 적었는데 잘못 적은거라서...
        {
            CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position);
            enemy.InstantKill(caster);
        }
        caster.AddStatusEffect(new Invincible(1, caster));
        MapManager.Inst.Swap(caster.GridPosition, position);
        foreach (var direction in MyUtil.Directions)
        {
            CharacterScript enemy = MapManager.Inst.GetCharacterAtPosition(position + direction);
            if (enemy != null)
            {
                enemy.TakeDamage(damage, caster);
            }
        }
    }
}
public class GuillotineEffectLogic : IEffectLogic
{
    public void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        if(MapManager.Inst.IsOccupied(target) == false)
        {
            caster.Move(target - caster.GridPosition);
            return;
        }
        int throwDistance = payload.Values[0];
        int damage = payload.Values[1];
        Vector2Int checkPosition = caster.GridPosition;
        Vector2Int direction = target - caster.GridPosition;
        CharacterScript targetEnemy = MapManager.Inst.GetCharacterAtPosition(target);

        caster.AddStatusEffect(new Invincible(1, caster));
        int count = 0;
        for (int i = 0; i < throwDistance; i++)
        {
            // 한칸씩 전진하면서
            // 벽을 만나면 던지는거리-이동거리 x 데미지 만큼 데미지

            // 어떻게 될지 한번 생각해보자...

            // case 1. 스킬을 쓰고 throwDistance 보다 더 가까이 벽이 있을 때
            // -> 벽을 만날 때 까지 적을 밀고 (throwDistance - 이동거리) * damage 만큼 데미지.
            // 만약 벽과 인접해 있는 상황이면? <- 여기에 대한 처리를 어떻게 해야할지?

            // case 2. 스킬을 썼는데 다른 적 개체와 부딪히게 될 경우
            // -> 마찬가지로 해당 진행방향 바로 전 칸에 배치하고 (throwDistance - 이동거리) * damage 만큼의 데미지를 양측에?
            // 이번에도 적과 인접해 있는 상황이면?

            // 추가로 적 AI도 이제 좀 나눠서 구현 할 수 있으면 좋겠는데
            // 원거리, 근거리, 자폭 대충 이렇게 세 가지 정도로 구분해서 적 AI를 나누고 싶다.

            checkPosition += direction;
            if(MapManager.Inst.IsWalkable(checkPosition) == false)
            {
                int modify = throwDistance - count;
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
public struct EffectPayload
{
    public EffectType Type;
    public int[] Values;
}
public interface IEffectLogic
{
    void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target);
}


public class SkillRecord
{
    public SkillData Data { get; private set; }
    public SkillType Type { get; private set; }
    public int CostSP { get; private set; }
    public List<EffectPayload> Effects { get; private set; }
    public SkillRecord(SkillData data)
    {
        Data = data;
        Effects = new List<EffectPayload>();
        CostSP = data.CostSP;
        foreach (var effectString in data.EffectList)
        {
            string[] effectParam = effectString.Split(':');
            int[] effectValues = new int[effectParam.Length - 1];
            if (effectParam.Length < 2)
            {
                continue;
            }
            for (int i = 1; i < effectParam.Length; i++)
            {
                if (int.TryParse(effectParam[i].Trim(), out int value) == false)
                {
                    continue;
                }
                effectValues[i - 1] = value;
            }
            if(Enum.TryParse(effectParam[0].Trim(),true, out EffectType effectType))
            {
                Effects.Add(new EffectPayload { Type = effectType, Values = effectValues });  
            }
        }
    }
    public void Execute(CharacterScript caster, Vector2Int target)
    {
        foreach (var payload in Effects)
        {
            EffectProcessor.ApplyEffect(payload, caster, target);
        }
    }
}

public class Skill // <- 말만 스킬인데 역할만 보면 그.. handler느낌이 드빈다, 그냥 레코드가 실행해도 상관없지 않나..? 
    // 그래서 이제 안씀... SkillRecord가 역할 대체함.
{
    public SkillRecord Record { get; private set; }
    public Skill(SkillRecord record)
    {
        Record = record;
    }
    public string Name { get { return Record.Data.Name; } }
    public string Description { get { return Record.Data.Description; } }
    public int CastRange { get { return Record.Data.CastRange; } }
    public void Execute(CharacterScript caster, Vector2Int target)
    {
        foreach(var payload in Record.Effects)
        {
            EffectProcessor.ApplyEffect(payload, caster, target);
        }
    }
}

public static class EffectProcessor
{
    public static readonly Dictionary<EffectType, IEffectLogic> _handlers = new Dictionary<EffectType, IEffectLogic>()
    {
        { EffectType.DAMAGE, new DamageEffectLogic() },
        { EffectType.HEAL, new HealEffectLogic() },
        { EffectType.SELFHEAL, new SelfHealEffectLogic() },
        { EffectType.DASHSLASH, new DashSlashEffectLogic() },
        { EffectType.WALLJUMP, new WallJumpEffectLogic() },
        { EffectType.POISONOUS, new PoisonousEffectLogic() }
    };
    public static void ApplyEffect(EffectPayload payload, CharacterScript caster, Vector2Int target)
    {
        if(_handlers.TryGetValue(payload.Type, out var effectLogic))
        {
            effectLogic.ApplyEffect(payload,caster, target);
        }
    }
}