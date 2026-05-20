using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BattleManager : MonoBehaviour
{
    private List<CharacterScript> _enemyList;
    private GameObject _enemyPrefab;
    public bool IsPlayerTurn { get; private set; }
    
    bool IsMyTurn(CharacterScript script)
    {
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
            targetCharacter.TakeDamage(1);
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

    private void Update()
    {
        if(IsPlayerTurn == false)
        {
            if (Input.anyKeyDown)
            {
                EnemyTurn();
                Input.ResetInputAxes();
                TurnChange();
                // AI가 일단은 저는... 그 원래 최단경로 자체는 보존시키고 싶고
                // 만약 가려는 길에 누군가가 서있으면 가려는 방향이 1,0 -> 1,1 | 1,0 | 1,-1 이렇게 세칸을 좀 비어있는 곳을 가고 싶다.
                // 1. 누군가가 서있으면 <- 검출이 안되고 있는 상황. 이래서 이러면 적들의 움직임을 하나씩 다라라라라 이동하고 그게 보이게 시키거나               
                // 2. 아니면... 따로 적들의 위치를 갖고 있는 데이터를 만들어서 거기서 한번 보게 시키거나... 해야할것 같습니다.
                // tile에 occupied를 갖고 있는게 좋다.
                // 점유하고 있는가?만 따지면 HashSet이기도 하고...
                // 점유중인 캐릭터를 알고 싶으면 Dictionary를 사용해야할 것 같습니다...
            }
        }
    }
    public void EnemyTurn()
    {
        foreach(var enemy in _enemyList)
        {
            var controller = enemy.gameObject.GetComponent<AIController>();
            controller.AITurn();
        }
    }

    public void TurnChange()
    {
        IsPlayerTurn = !IsPlayerTurn;
        GameObject mainUI = PracticeUIManager.Inst.GetCreatedUI(UIType.MainUI);
        mainUI.GetComponent<PracticeMainUI>().SetCurrentTurn(IsPlayerTurn);
        Debug.Log($"Turn Changed : {IsPlayerTurn}");
    }

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
