using System;
using UnityEngine;

public class LobbyScreen : MonoBehaviour
{
    [SerializeField] private SelectionLayout _selectPanel;
    private void OnEnable()
    {
        _selectPanel.BindStartButtonEvent(StartGame);
    }
    private void OnDisable()
    {
        _selectPanel.UnBindStartButtonEvent(StartGame);
    }
    void StartGame()
    {
        //가라 오프닝
        Destroy(this.gameObject);
    }
}
