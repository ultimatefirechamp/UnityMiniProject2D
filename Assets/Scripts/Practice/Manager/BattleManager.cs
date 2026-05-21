using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BattleManager : MonoBehaviour
{
    private List<CharacterScript> _enemyList;
    private CharacterScript _player;
    private GameObject _enemyPrefab;
    private PracticeMainUI _mainUI;
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
        _mainUI = PracticeUIManager.Inst.GetCreatedUI(UIType.MainUI).GetComponent<PracticeMainUI>();

        // 디버깅용으로 편의상 로드해둔 데이터들
        _enemyPrefab = PracticeResourceManager.Inst.LoadPrefab("Prefabs/Practice_KCK/Character/Enemy");
        // 실제 인게임 상에서 배틀 매니저가 enemy 스폰에 직접적 관여 금지
        if(GameDataManager.Instance.GetSkill("skill_smash") == null)
        {
            Debug.LogWarning("Skill Data NULL");
        }
        // 스킬도 확인 용으로 임시로 받은 데이터.
        Debug.Log((GameDataManager.Instance.GetSkill("skill_smash")).Name);
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
            targetCharacter.TakeDamage(1, attacker);
        }
    }

    public void RequestSkill(CharacterScript caster, Vector2Int target, Skill skill)
    {
        // skill check and execute Skill
        if(IsMyTurn(caster) == false)
        {
            return;
        }

        skill.Execute(caster,target);
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
        _mainUI.SetCurrentTurn(IsPlayerTurn);
        if(IsPlayerTurn)
        {
            //_player.OnTurnStart();
        }
        else
        {
            EnemyTurn();
        }
    }
    void ProcessTick()
    {
        while(_player.AP < _player.APCost)
        {
            _player.AddActionPoint(100);
            foreach(var enemy in _enemyList)
            {
                enemy.AddActionPoint(100);
                if(enemy.AP >= enemy.APCost)
                {
                    var controller = enemy.gameObject.GetComponent<AIController>();
                    controller.AITurn();
                    enemy.OnActionEnd();
                }
            }
        }
        Input.ResetInputAxes();
    }
    // 이하는 디버그 용으로 만들었던 SpawnEnemy 메서드. 나중에 Spawn전용 클래스를 만들거나 ObjectManager를 두면 좋을 듯.
    public void SpawnEnemy()
    {
        GameObject spawnedEnemy = Instantiate(_enemyPrefab);
        spawnedEnemy.transform.position = new Vector2(0.5f, 0.5f);
        _enemyList.Add(spawnedEnemy.GetComponent<CharacterScript>());
    }
    
    public CharacterScript GetSpawnEnemy()
    {
        GameObject spawnedEnemy = Instantiate(_enemyPrefab);
        spawnedEnemy.transform.position = new Vector2(0.5f, 0.5f);
        _enemyList.Add(spawnedEnemy.GetComponent<CharacterScript>());
        return spawnedEnemy.GetComponent<CharacterScript>();
    }
}
