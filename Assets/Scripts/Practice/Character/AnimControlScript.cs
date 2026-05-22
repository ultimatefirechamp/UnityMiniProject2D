using UnityEngine;
public enum CharacterStates
{
    IDLE,
    WALK,
    RUN,
    ATTACK
}

public class AnimControlScript : MonoBehaviour
{
    
    private CharacterStates _currentState;
    [SerializeField] private Animator _animator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimationInput();
    }
    void AnimationInput()
    {
        ResetAllAnimParameter();
        if (Input.GetKey(KeyCode.W))
        {
            SetState(CharacterStates.WALK);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SetState(CharacterStates.RUN);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetState(CharacterStates.ATTACK);
        }
    }
    public void SetState(CharacterStates newState)
    {
        if(newState == CharacterStates.IDLE && _currentState == CharacterStates.IDLE)
        {
            return;
        }
        _currentState = newState;

        switch (_currentState)
        {
            case CharacterStates.IDLE:
                {
                    ResetAllAnimParameter();
                    break;
                }
            case CharacterStates.WALK:
                {
                    _animator.SetBool("isWalk", true);
                    _animator.SetBool("isRun", false);
                    break;
                }
            case CharacterStates.RUN:
                {
                    _animator.SetBool("isRun", true);
                    break;
                }
            case CharacterStates.ATTACK:
                {
                    _animator.SetTrigger("Attack");
                    break;
                }
            default:
                {
                    ResetAllAnimParameter();
                    break;
                }
        }
    }
    void ResetAllAnimParameter()
    {
        _animator.SetBool("isWalk", false);
        _animator.SetBool("isRun", false);
    }
}
