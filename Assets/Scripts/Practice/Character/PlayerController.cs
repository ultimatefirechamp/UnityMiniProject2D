using UnityEngine;
public interface IControllable
{
    void Move(Vector2Int direction);
    void TakeDamage(int damage, CharacterScript attacker);
    void Attack(Vector2Int target);
}
public class PlayerController : MonoBehaviour
{
    private IControllable _character;
    private void Awake()
    {
        FindControllableComponent();
    }
    void FindControllableComponent()
    {
        if(this.gameObject.TryGetComponent<IControllable>(out var controllerable))
        {
            _character = controllerable;
            return;
        }
        Debug.LogWarning($"{this.gameObject.name} : Can't Find Controllable Component.");
    }
    void Update()
    {
        MoveInputHandle();
        //SkillInputHandle();
    }
    void MoveInputHandle()
    {
        if(BattleManager.Inst.IsPlayerTurn == false)
        {
            return;
        }
        if(Input.anyKeyDown == false)
        {
            return;
        }
        int x = 0;
        int y = 0;
        if (Input.GetKeyDown(KeyCode.W))
        {
            y += 1;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            y -= 1;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            x += 1;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            x -= 1;
            y += 1;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            x += 1;
            y += 1;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            x -= 1;
            y -= 1;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            x += 1;
            y -= 1;
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            // 마음에 안드는 매직 넘버... 일단 대기인지 입력이 없는건지 구분하기 위해 이렇게 함...
            x = -2;
            y = -2;
        }
        Vector2Int moveDirection = new Vector2Int(x, y);
        
        if (moveDirection == Vector2.zero)
        {
            return;
        }

        if(moveDirection == new Vector2(-2,-2))
        {
            BattleManager.Inst.ProcessTick();
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(_character is CharacterScript player)
            {
                player.UseSkill("skill_flyingswallow", moveDirection + player.GridPosition);
            }
        }
        else
        {
            _character.Move(moveDirection);
        }

        BattleManager.Inst.ProcessTick();
    }
    void SkillInputHandle()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if(_character is CharacterScript playableCharacter)
            {
                // Test Skill Script
                playableCharacter.Skill(playableCharacter.GridPosition+Vector2Int.up);
            }
        }
    }
}
