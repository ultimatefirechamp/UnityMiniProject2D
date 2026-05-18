using UnityEngine;

public class PracticeMainUI : MonoBehaviour
{
    [SerializeField] PracticeTopUI topUI;

    public void SetCurrentTurn(bool currentTurn)
    {
        topUI.setCurrentTurn(currentTurn);
    }
    
    
}