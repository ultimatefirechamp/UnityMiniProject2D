using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class StatusEffectDataModel
{
    public string Name;
    public int EffectStack;
    public int EffectDuration;
}
public class SkillDataModel
{
    public string Id;
}
public class TraitDataModel
{
    public string TraitName;
}
public class CharacterDataModel
{
    public string Id;
    public Vector2Int Position;
    public string Name;
    public int CurrentHP;
    public int CurrentSP;
    public List<StatusEffectDataModel> StatusEffectList; 
    public List<SkillData> SkillList;
    public List<TraitDataModel> TraitList; 
}
public class WorldDataModel
{
    public List<CharacterDataModel> EnemyList = new List<CharacterDataModel>();
    public CharacterDataModel Player = new CharacterDataModel();
    public int WorldTick;
    public int WorldTickInterval;
    public string CurrentMap;
}
