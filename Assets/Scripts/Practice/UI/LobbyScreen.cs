using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    [SerializeField] private SelectionLayout _selectPanel;
    private void OnEnable()
    {
        _selectPanel.BindStartButtonEvent(StartGame);
        _selectPanel.BindQuitButtonEvent(QuitGame);
    }
    private void OnDisable()
    {
        _selectPanel.UnBindStartButtonEvent(StartGame);
        _selectPanel.UnBindQuitButtonEvent(QuitGame);
    }
    void StartGame()
    {
        //가라 오프닝
        //Debug.Log("start");
        GameFlowManager.Inst.SetGamePhase(GamePhase.Ingame);
    }
    void QuitGame()
    {
        Application.Quit();
    }
    
}
