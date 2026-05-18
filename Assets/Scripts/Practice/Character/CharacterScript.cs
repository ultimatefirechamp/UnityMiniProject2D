using System;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterScript : MonoBehaviour, IControllable
{
    public event Action<Transform> OnMove;
    public event Action<int,int> OnDamaged;
    public int MaxHp { get; private set; }
    public int Hp { get; private set; }
    public int AC { get; private set; }
    public bool IsAlive { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    private bool _drawCheck = false;
    public int AttackRange { get; private set; }
    private void Awake()
    {
        MaxHp = 10;
        Hp = 10;
        IsAlive = true;
    }
    private void Start()
    {
        Init();
        GridPosition = MapManager.Inst.WorldToArrayPos(transform.position);
        MapManager.Inst.OccupyTile(GridPosition, this);
    }
    void Init()
    {
        HPBarGroupScript hpbarGroup = PracticeUIManager.Inst.GetCreatedUI(UIType.HPBarGroup).GetComponent<HPBarGroupScript>();
        // register this character to Ui
        hpbarGroup.RegisterCharacter(this);
    }
    private void Update()
    {
    }
    public void Move(Vector2Int direction)
    {
        // Request to Manager
        Vector2Int prevPos = GridPosition;
        Vector2Int destPos = GridPosition + direction;
        if (MapManager.Inst.IsOccupied(destPos))
        {
            Debug.Log($"{destPos} IsOccupied");
            return;
        }
        transform.position = (Vector2)transform.position + direction;
        GridPosition = destPos;
        MapManager.Inst.MoveTo(prevPos, destPos, this);
        OnMove?.Invoke(this.transform);
    }
    public void Attack(Vector2Int target)
    {
        BattleManager.Inst.RequestAttack(this, target);
    }
    public void TakeDamage(int damage)
    {
        Hp -= damage;
        OnDamaged?.Invoke(MaxHp,Hp);
        if (Hp <= 0)
        {
            IsAlive = false;
        }
    }
    private void OnDestroy()
    {
        HPBarGroupScript hpbarGroup = PracticeUIManager.Inst.GetCreatedUI(UIType.HPBarGroup).GetComponent<HPBarGroupScript>();
        hpbarGroup.UnRegisterCharacter(this);
    }


}
