using UnityEngine;
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
public class Effect
{
    EffectType type;
    int value;
    public void ApplyEffect(CharacterScript caster, List<CharacterScript> opponents)
    {
        foreach(var target in opponents)
        {
            switch(type)
            {
                case EffectType.DAMAGE:
                    target.TakeDamage(value);
                    break;
                case EffectType.HEAL:
                    caster.TakeDamage(-value);
                    break;
                case EffectType.MOVE:
                    caster.Move(Vector2Int.down); // 임시. 특정 방향으로 value만큼 이동하는 로직 만들어야 함.
                    break;
                case EffectType.KNOCKBACK:
                    target.Move(Vector2Int.up); // 임시. 특정 방향으로 value만큼 이동하는 로직 만들어야 함.
                    break;
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
    public string EffectList;
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
    public void Execute(CharacterScript caster, List<CharacterScript> opponents)
    {
        foreach(var effect in _effectList)
        {
            effect.ApplyEffect(caster, opponents);
        }
    }
}