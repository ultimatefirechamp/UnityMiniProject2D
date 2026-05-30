using System.Collections.Generic;
using UnityEngine;

public class SkillSystem
{
    Dictionary<string, SkillRecord> _skillList;
    Dictionary<SkillComboType, string> _skillMaping;
    CharacterScript _owner;


    public SkillSystem(CharacterScript owner)
    {
        _owner = owner;
        _skillList = new Dictionary<string, SkillRecord>();
        _skillMaping = new Dictionary<SkillComboType, string>();
        AddSkill("skill_flyingswallow", SkillComboType.SHIFT);
        AddSkill("skill_walljump");
    }

    public void UseSkill(string skillId, Vector2Int target)
    {
        if (_skillList.TryGetValue(skillId, out var skill) == false)
        {
            return;
        }
        if (skill == null)
        {
            Debug.LogWarning($"{_owner.name} doesn't have {skillId} skill");
            return;
        }
        if(skill.CostSP > _owner.SP)
        {
            Debug.Log("not enough SP");
            return;
        }
        _owner.ReduceSP(skill.CostSP);
        BattleManager.Inst.RequestSkill(_owner, target, skill);
    }
    public void UseSkill(SkillComboType comboType, Vector2Int target)
    {
        if(_skillMaping.TryGetValue(comboType, out var skillId))
        {
            UseSkill(skillId, target);
        }
    }
    public void AddSkill(string skillId, SkillComboType comboType = SkillComboType.NONE)
    {
        _skillList.Add(skillId, GameDataManager.Instance.GetSkillRecord(skillId));
        if(comboType == SkillComboType.NONE)
        {
            return;
        }
        _skillMaping.Add(comboType, skillId);
    }
}