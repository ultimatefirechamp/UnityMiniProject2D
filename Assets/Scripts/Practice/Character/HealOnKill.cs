using UnityEngine;

public class HealOnKill : MonoBehaviour
{
    private CharacterScript _character;
    private int _healAmount = 5;

    private void OnEnable()
    {
        if(gameObject.TryGetComponent<CharacterScript>(out CharacterScript character))
        {
            _character = character;
            _character.OnKillEvent += HandleKill;
            Debug.Log($"{_character.name} OnKillEvent Bind Complete");
        }
    }
    private void HandleKill(CharacterScript victim)
    {
        _character.Heal(_healAmount);
        Debug.Log($"HealEvent");
    }
}
