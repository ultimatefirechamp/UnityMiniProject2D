using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BattleManager : MonoBehaviour
{
    private List<CharacterScript> _enemyList;
    public CharacterScript _player; // 원래 이러면 안되지만... 아 귀찮다... 일단 직접 연결하고 나중에 따로 불러오도록 하는걸로
    public int WorldTick { get; private set; } = 0;
    public int WorldTickInterval { get; private set; } = 100;
    public int WorldTickIncrease { get; private set; } = 100;

    // 현재 구상중인 방식이
    // World에 틱이 interval만큼 올라가면서 

    public bool IsPlayerTurn { get; private set; }
    
    bool IsMyTurn(CharacterScript script)
    {
        //return script.AP >= script.APCost;

        if(script.CompareTag("Player") == IsPlayerTurn)
        {
            return true;
        }
        return false;
    }
    
    private static BattleManager _instance;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        IsPlayerTurn = true;
        _enemyList = new List<CharacterScript>();
    }
    
    private void Start()
    {

    }
    
    public static BattleManager Inst { get { return _instance; } }
    
    public void RegistEnemy(CharacterScript enemy)
    {
        _enemyList.Add(enemy);
    }
    public void RequestAttack(CharacterScript attacker, Vector2Int target)
    {
        var targetCharacter = MapManager.Inst.GetCharacterAtPosition(target);
        if (targetCharacter == null)
        {
            Debug.LogWarning($"There is no character at {target}");
            return;
        }
        var attackerPosition = attacker.GridPosition;

        int attackRangeSqr = attacker.AttackRange * attacker.AttackRange;
        int distance = MyUtil.GetDistance(attackerPosition, target);
        if (distance <= attacker.AttackRange)
        {
            // 추후에 방어력이나 데미지 계산 요소도 넣어야 할텐데...
            // TakeDamage는 순수 공격수치만 넣고
            // 데미지 받는 쪽 내부에서 자신의 AC수치랑 함께 계산해서 하는걸로 해야하나.
            targetCharacter.TakeDamage(attacker.ATK, attacker);
        }
    }
    public void RequestSkill(CharacterScript caster, Vector2Int target, SkillRecord skill)
    {
        // skill check and execute Skill
        //if(IsMyTurn(caster) == false)
        //{
        //    return;
        //}

        skill.Execute(caster,target);
    }

    public void SetPlayer(CharacterScript player)
    {
        _player = player;
    }

    void TurnStart()
    {
        if(IsPlayerTurn)
        {
            _player.OnTurnStart();
        }
        else
        {
            foreach (var enemy in _enemyList)
            {
                enemy.OnTurnStart();
            }
        }
    }
    void TurnEnd()
    {
        if (IsPlayerTurn)
        {
            _player.OnActionEnd();
        }
        else
        {
            foreach (var enemy in _enemyList)
            {
            }
        }
    }
 
    public void EnemyTurn()
    {
        TurnStart();
        foreach(var enemy in _enemyList)
        {
            var controller = enemy.gameObject.GetComponent<AIController>();
            if(controller != null)
            {
                controller.AITurn();
                enemy.OnActionEnd();
            }
        }
        Input.ResetInputAxes();
        TurnChange();
    }

    public void TurnChange()
    {
        IsPlayerTurn = !IsPlayerTurn;
        if(IsPlayerTurn)
        {
            //_player.OnTurnStart();
        }
        else
        {
            EnemyTurn();
        }
    }

    public void ProcessTick()
    {
        // ProcessTick에 진입했다는 것은 이미 플레이어턴이 한번은 진행되었다는 것을 내포함.
        _player.OnActionEnd();
        RefreshEnemyList();

        //턴 진행 순서.
        // 월드 
        // ->
        // 플레이어
        // -> 
        // 몬스터
        // -> 월드
        // 지금은 인터벌에 비해 본인 속도가 과하게 빠른애가 여러마리면
        // 남들도 행동을 할 수는 있지만 일단 본인 먼저 루프를 돌면서 처리하는 경향있음
        // 좀 더 공평하게 분배할 수는?
        // Process턴 안에서 루프를 돌고 있음.
        // Player의 Tick이 다 찰 때 까지 루프를 도는 구조.
        // 코드의 순서 자체는 플레이어 -> 몬스터 -> 월드 -> 플레이어 ->.. 인데
        // 어차피 루프라서 그게 구별 가능하지는 않을 듯.

        while (_player.AP < _player.APCost)
        {
            // player의 AP를 채우기만하고 별다른 검사를 하지 않아도 괜찮을 듯.
           
            foreach(var enemy in _enemyList)
            {
                // 현재 IsAlive를 건들거나 사용하는 로직을 짜지 않아서 isAlive판단 아직 안함
                // 죽었을 때 행동을 넘기거나 하는건 AITurn내부 컨트롤러 같은 곳에서 판단?
                // 죽었을 때 바로 Destroy시키지 말고 후에 턴 종료라고 선언되는 시점마다 리스트 정리하도록 시킬 예정.
                // 턴종료 시점? ... 아마 ProcessTick 시작할때 (플레이어 턴 종료), ProcessTick 맨 아래 (AI턴 종료) 이렇게 두번..?
                // Player가 두번 연속으로 행동할 수 있으면 그냥 리스트 정리가 몇번 연속으로 실행되기는 할 텐데...
                // 아 몰라. 근데 얘 몇번이나 돌아가든 죽은 애들만 걸러내는 거니까 쓸데없는 계산 몇번하는 정도의 문제만 있지 큰 문제는 없겠지
                enemy.AddActionPoint(WorldTickInterval);
                while(enemy.AP >= enemy.APCost)
                {
                    var controller = enemy.gameObject.GetComponent<AIController>();
                    enemy.OnTurnStart();
                    if (enemy.IsAlive == false)
                    {
                        break;
                    }
                    controller.AITurn();
                    enemy.OnActionEnd();
                }
            }
            _player.AddActionPoint(WorldTickInterval);
            WorldTick += WorldTickInterval;
            if (WorldTick >= WorldTickInterval)
            {
                OnTick();
                WorldTick -= WorldTickInterval;
            }
        }
        RefreshEnemyList();
        if(_player.IsAlive == false)
        {
            GameFlowManager.Inst.SetGamePhase(GamePhase.GameOver);
            //GameOverScreen;
        }
    }

    void OnTick()
    {
        _player.OnWorldTick();
        foreach (var enemy in _enemyList)
        {
            enemy.OnWorldTick();
        }
    }    
    void RefreshEnemyList()
    {
        for(int i = _enemyList.Count-1; i >= 0; i--)
        {
            if (_enemyList[i] == null || _enemyList[i].IsAlive == false)
            {
                Destroy(_enemyList[i].gameObject);
                _enemyList.RemoveAt(i);
            }
        }
    }
    
    public void ResetManager()
    {
        WorldTick = 0;
        _player = null;
        _enemyList.Clear();
    }
}
