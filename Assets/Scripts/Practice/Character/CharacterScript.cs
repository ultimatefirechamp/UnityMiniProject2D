using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterScript : MonoBehaviour, IControllable
{
    public event Action<Transform> OnMove;
    public event Action<int,int> OnDamaged;
    private Skill testingSkill;
    public event Action<CharacterScript> OnKillEvent;
    Dictionary<string, Skill> _skillList;

    public int MaxHp { get; private set; } = 10;
    public int Hp { get; private set; } = 10;
    public int AC { get; private set; } = 0;
    public int AP { get; private set; } = 0;
    public int APCost { get; private set; } = 100;
    public int APCostDefault { get; private set; } = 100;
    public int ATK { get; private set; } = 1;
    public bool IsAlive { get; private set; } = true;
    public Vector2Int GridPosition { get; private set; }
    Dictionary<string, StatusEffect> _statusList;
    public int AttackRange { get; private set; } = 1;
    public string Name { get; private set; } = "popoi";
    private void Awake()
    {
        MaxHp = 10;
        Hp = 10;
        IsAlive = true;
        AttackRange = 1;
        _skillList = new Dictionary<string, Skill>();
    }
    private void Start()
    {
        Init();
        GridPosition = MapManager.Inst.WorldToArrayPos(transform.position);
        MapManager.Inst.OccupyTile(GridPosition, this);
        testingSkill = SkillFactory.CreateSkill(GameDataManager.Instance.GetSkill("skill_flyingswallow"));
        _skillList["skill_flyingswallow"] = testingSkill;
        _statusList = new Dictionary<string, StatusEffect>();

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
    }
    public void Skill(Vector2Int target)
    {
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
    public void Move(Vector2Int direction)
    {
        // Request to Manager
        Vector2Int prevPos = GridPosition;
        Vector2Int destPos = GridPosition + direction;
        if (MapManager.Inst.IsOccupied(destPos) || MapManager.Inst.IsWalkable(destPos) == false)
        {
            return;
        }
        MapManager.Inst.MoveTo(prevPos, destPos, this);
    }
    public void Attack(Vector2Int target)
    {
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
    public void InstantKill()
    {
        Hp = 0;
        IsAlive = false;
    }
    public void TakeDamage(int damage, CharacterScript attacker = null)
    {
        Hp -= damage;
        Debug.Log($"{gameObject.name} take {damage} damage");
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
        List<string> removeList = new List<string>();
        foreach(var status in _statusList.Values)
        {
            status.OnTurnTick();
            if(status.Stack == 0 || status.Duration == 0)
            {
                removeList.Add(status.Name);
            }
        }
        foreach(var status in removeList)
        {
            _statusList.Remove(status);
        }
    }
    public void OnActionEnd()
    {
        AP -= APCost;
    }
    private void OnDisable()
    {
        MapManager.Inst.ClearTile(GridPosition);
        HPBarGroupScript hpbarGroup = PracticeUIManager.Inst.GetCreatedUI(UIType.HPBarGroup).GetComponent<HPBarGroupScript>();
        hpbarGroup.UnRegisterCharacter(this);
    }
    private void OnDestroy()
    {
    }
}
