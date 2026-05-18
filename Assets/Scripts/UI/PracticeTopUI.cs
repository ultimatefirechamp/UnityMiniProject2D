using UnityEngine;
using UnityEngine.UI;

public class PracticeTopUI : MonoBehaviour
{
    [SerializeField] private Text currentTurn;

    public void setCurrentTurn(bool state)
    {
        string turn = string.Empty;
        if (state)
        {
            turn = "Player Turn";
        }
        else
        {
            turn = "Enemy Turn";
        }
        currentTurn.text = turn;
    }
}
