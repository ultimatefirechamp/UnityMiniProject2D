using UnityEngine;

public class HealOnKill : MonoBehaviour
{
    private CharacterScript _character;
    private int _healAmount = 3;

    private void OnEnable()
    {
        _character = GetComponent<CharacterScript>();
        _character.OnKillEvent += HandleKill;
    }
    private void HandleKill(CharacterScript victim)
    {
        _character.Heal(_healAmount);
    }
}
