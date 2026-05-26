using AutoGroupGenerator;
using UnityEngine;
using UnityEngine.UI;

public class DebugInputField : MonoBehaviour
{
    [SerializeField] InputField _commandInputField;
    
    void ProcessCommand(string input)
    {
        string[] args = input.Split(' ');
        string command = args[0].ToLower();

        switch(command)
        {
            case "spawn":
                break;
            case "addeffect":
                break;
        }
    }
    void SpawnEnemy()
    {

    }
    void AddEffect()
    {

    }
    void MovePosition()
    {

    }
}
