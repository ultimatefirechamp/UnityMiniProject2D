using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterScript))]
public class SkillComponent : MonoBehaviour
{
    Dictionary<string, SkillRecord> _skillList;
    Dictionary<SkillComboType, string> _skillMaping;
    CharacterScript _owner;
    bool _isSet = false;

    private void Awake()
    {
        if(gameObject.TryGetComponent<CharacterScript>(out _owner) == false)
        {
            return;
        }
        _isSet = true;
        _skillList = new Dictionary<string, SkillRecord>();
        _skillMaping = new Dictionary<SkillComboType, string>();
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
        BattleManager.Inst.RequestSkill(_owner, target, skill);
    }
    public void UseSkill(SkillComboType comboType, Vector2Int target)
    {
        if(_isSet == false)
        {
            Debug.LogWarning("Can't Find CharacterScript!");
            return;
        }
        if(_skillMaping.TryGetValue(comboType, out var skillId))
        {
            UseSkill(skillId, target);
        }
    }

}
