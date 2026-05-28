using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackData
{
    public CharacterScript Attacker;
    public int Damage;
    public bool IsEvaded;
    public AttackData(CharacterScript attacker, int damage)
    {
        Attacker = attacker;
        Damage = damage;
        IsEvaded = false;
    }
}
public class CharacterScript : MonoBehaviour, IControllable
{
    // 강사님 조언.
    // 캐릭터에게 달린 컴포넌트가 너무 많다. 통합하거나 아니면 순수 C# 클래스로 뺄 수 있는 내용은 없을까?

    // 캐릭터 행위에 관한 이벤트
    public event Action<Transform> OnMove;
    public event Action OnAttack; // 일단 만들기만 해둠. 후에 필요한 일이 생기면 매개변수와 함께 쓸 예정.
    public event Action<CharacterScript> OnKillEvent;
    public event Action OnSkill; // 일단 만들기만 해둠. 후에 필요한 일이 생기면 매개변수와 함께 쓸 예정.
    
    // 데미지와 관련한 이벤트
    public event Action<AttackData> OnBeforeDamage;
    public event Action<AttackData> OnDamageStep;
    public event Action<int, int> OnDamaged;

    // 턴 주기와 관련한 이벤트
    public event Action OnTickStart;

    // 상태이상 관련 이벤트
    public event Action<StatusEffect> OnAddEffect;

    // 미분류 이벤트
    public event Action OnCharacterDestroy;
    public event Action<int, int> OnSpChanged;

    private SkillRecord testingSkill;

    // 처음에는 스킬, 특성, 상태이상을 캐릭터 스크립트에 두지 않으려 했는데
    // 무엇을 가지고 있는지 정도는 캐릭터가 알고 있고 로직을 다른 곳으로 빼는걸로...
    // 변천사
    // 캐릭터가 로직까지 들고 있음 -> 캐릭터가 아무것도 모르고 있음 -> 내가 뭘 가지고 있는지는 캐릭터가 알고 있음.   
    Dictionary<string, SkillRecord> _skillList;
    SkillSystem _skillComp;
    public StatusEffectSystem EffectSystem { get; private set; }
    List<Trait> _activeTraits;

    public int MaxHp { get; private set; } = 10;
    public int Hp { get; private set; } = 10;
    public int AC { get; private set; } = 0;
    public int AP { get; private set; } = 0;
    public int APCost { get; private set; } = 100;
    public int APCostDefault { get; private set; } = 100;
    public int SP { get; private set; } = 10;
    public int MaxSP { get; private set; } = 10;
    public int ATK { get; private set; } = 1;
    public bool IsAlive { get; private set; } = true;
    public Vector2Int GridPosition { get; private set; }
    public int AttackRange { get; private set; } = 3;
    public string Name { get; private set; } = "popoi";
    private void Awake()
    {
        _skillList = new Dictionary<string, SkillRecord>();
        _activeTraits = new List<Trait>();
        EffectSystem = new StatusEffectSystem(this);
    }
    private void Start()
    {
        Init();
        EffectSystem.Init();
        _activeTraits.Add(new HealOnKillTrait());
        _activeTraits[0].Equip(this);
        GridPosition = MapManager.Inst.WorldToArrayPos(transform.position);
        MapManager.Inst.OccupyTile(GridPosition, this);
        SkillRecord record = GameDataManager.Instance.GetSkillRecord("skill_flyingswallow");
        testingSkill = record;
        _skillList["skill_flyingswallow"] = testingSkill;
        _skillComp = new SkillSystem(this);
        //StatusEffect invincible = new Invincible(99, this);
        //_statusList.Add(invincible.Id, invincible);
    }
    void Init()
    {
        HPBarGroupScript hpbarGroup = PracticeUIManager.Inst.GetCreatedUI(UIType.HPBarGroup).GetComponent<HPBarGroupScript>();
        // register this character to Ui
        hpbarGroup.RegisterCharacter(this);
    }
    public void SetCharacter(MonsterData data)
    {
        MaxHp = data.HP;
        Hp = data.HP;
        Name = data.Name;
        AttackRange = data.Range;
        ATK = data.ATK;
        AC = data.AC;
        OnDamaged?.Invoke(MaxHp,Hp);
    }
    public void Skill(Vector2Int target)
    {
        OnSkill?.Invoke();
        BattleManager.Inst.RequestSkill(this, target, testingSkill);
        //testingSkill.Execute(this, target);
    }
    public void UseSkill(string skillName, Vector2Int target)
    {
        if(_skillList.TryGetValue(skillName, out var skill))
        {
            BattleManager.Inst.RequestSkill(this, target, skill);
            return;
        }
        Debug.LogWarning($"This Character don't have {skillName} {gameObject.name}");
    }
    public void UseSkill(SkillComboType comboType, Vector2Int moveDirection)
    {
        if(_skillComp == null)
        {
            return;
        }
        Vector2Int target = moveDirection + GridPosition;
        _skillComp.UseSkill(comboType, target);
    }
    public void Move(Vector2Int direction)
    {
        // Request to Manager
        Vector2Int prevPos = GridPosition;
        Vector2Int destPos = GridPosition + direction;
        if (MapManager.Inst.IsWalkable(destPos) == false)
        {
            _skillComp.UseSkill("skill_walljump", destPos);
            return;
        }
        if(MapManager.Inst.IsOccupied(destPos))
        {
            CharacterScript target = MapManager.Inst.GetCharacterAtPosition(destPos);
            if(target == this)
            {
                return;
            }
            Attack(destPos);
        }
        MapManager.Inst.MoveTo(prevPos, destPos, this);
    }
    public void Attack(Vector2Int target)
    {
        OnAttack?.Invoke();
        BattleManager.Inst.RequestAttack(this, target);
    }
    public void Heal(int healAmount)
    {
        Hp += healAmount;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
        OnDamaged?.Invoke(MaxHp, Hp);
    }
    public void InstantKill(CharacterScript attacker = null)
    {
        Hp = 0;
        IsAlive = false;
        if (attacker != null)
        {
            attacker.NotifyKill(this);
        }
    }
    public void TakeDamage(int damage, CharacterScript attacker = null)
    {
        AttackData attackData = new AttackData(attacker, damage);

        OnBeforeDamage?.Invoke(attackData);

        if(attackData.IsEvaded)
        {
            return;
        }

        OnDamageStep?.Invoke(attackData);

        int modifiedDamage = attackData.Damage;
        Hp -= modifiedDamage;
        Debug.Log($"{gameObject.name} take {modifiedDamage} damage");
        OnDamaged?.Invoke(MaxHp,Hp);
        if (Hp <= 0)
        {
            IsAlive = false;
            if(attacker != null)
            {
                attacker.NotifyKill(this);
            }
        }
    }
    public void NotifyKill(CharacterScript victim)
    {
        OnKillEvent?.Invoke(victim);
    }
    public void AddActionPoint(int value)
    {
        AP += value;
    }

    /// <summary>
    /// 물리적인 위치 옮기는 부분도 포함되어 있음.
    /// </summary>
    public void SetGridPosition(Vector2Int position)
    {
        GridPosition = position;
        transform.position = MapManager.Inst.ArrayToWorldPos(position.x, position.y);
        OnMove?.Invoke(this.transform);
    }
    public void OnTurnStart()
    {
    }
    public void OnWorldTick()
    {
        OnTickStart?.Invoke();
    }
    public void OnActionEnd()
    {
        AP -= APCost;
    }
    public void AddStatusEffect(StatusEffect effect)
    {
        OnAddEffect?.Invoke(effect);
    }
    public void CommandSkill(SkillComboType comboType, Vector2Int direction)
    {
        if(_skillComp == null)
        {
            Debug.LogWarning("{gameObject.name} doesn't have SkillComponent");
            return;
        }
        _skillComp.UseSkill(comboType, direction + GridPosition);
    }

    private void OnDisable()
    {
        if(PracticeUIManager.Inst != null)
        {
            HPBarGroupScript hpbarGroup = PracticeUIManager.Inst.GetCreatedUI(UIType.HPBarGroup).GetComponent<HPBarGroupScript>();
            if(hpbarGroup != null)
            {
                hpbarGroup.UnRegisterCharacter(this);
            }
        }
        MapManager.Inst.ClearTile(GridPosition);
    }
    private void OnDestroy()
    {
        OnCharacterDestroy?.Invoke();
        EffectSystem.CleanUp();
        foreach(var trait in _activeTraits)
        {
            trait.UnEquip();
        }
        _activeTraits.Clear();
    }
}
