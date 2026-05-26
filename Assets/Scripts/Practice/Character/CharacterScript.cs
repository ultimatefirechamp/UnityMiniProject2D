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
    Dictionary<string, SkillRecord> _skillList;
    SkillComponent _skillComp;

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
        _skillComp = gameObject.GetComponent<SkillComponent>();
    }
    private void Start()
    {
        Init();
        GridPosition = MapManager.Inst.WorldToArrayPos(transform.position);
        MapManager.Inst.OccupyTile(GridPosition, this);
        SkillRecord record = GameDataManager.Instance.GetSkillRecord("skill_flyingswallow");
        testingSkill = record;
        _skillList["skill_flyingswallow"] = testingSkill;
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
    }
}
