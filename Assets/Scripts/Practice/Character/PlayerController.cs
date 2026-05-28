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
    private void Start()
    {
        if(_character is CharacterScript player)
        {
            BattleManager.Inst.SetPlayer(player);
        }
    }
    void Update()
    {
        InputHandle();
    }
    void InputHandle()
    {
        bool isWait = false;
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
        SkillComboType comboKey = SkillComboType.NONE;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            y += 1;
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            y -= 1;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            x -= 1;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
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
            isWait = true;
        }
        Vector2Int moveDirection = new Vector2Int(x, y);

        if (isWait == true)
        {
            BattleManager.Inst.ProcessTick();
            return;
        }

        if (moveDirection == Vector2.zero)
        {
            return;
        }

        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            comboKey = SkillComboType.SHIFT;
            //if(_character is CharacterScript player)
            //{
            //    //player.UseSkill("skill_flyingswallow", moveDirection + player.GridPosition);
            //    player.UseSkill(SkillComboType.SHIFT, moveDirection);
            //}
        }

        if(Input.GetKey(KeyCode.LeftControl))
        {
            comboKey = SkillComboType.CTRL;
        }

        if(comboKey == SkillComboType.NONE)
        {
            _character.Move(moveDirection);
        }
        else
        {
            if(_character is CharacterScript player)
            {
                player.UseSkill(comboKey, moveDirection);
            }
        }

        BattleManager.Inst.ProcessTick();
    }
    
}
