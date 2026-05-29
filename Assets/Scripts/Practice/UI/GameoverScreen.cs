using System.Collections.Generic;
using UnityEngine;

public class GameoverScreen : MonoBehaviour
{
    [SerializeField] ButtonBase _retryButton;
    [SerializeField] ButtonBase _giveupButton;
    int _cursor;
    int _length;
    List<ButtonBase> _btnList = new List<ButtonBase>();

    private void OnEnable()
    {
        _retryButton.BindOnClickEvent(RetryGame);
        _giveupButton.BindOnClickEvent(GiveUp);
        _btnList.Add(_retryButton);
        _btnList.Add(_giveupButton);
        _cursor = 0;
        _length = 2;
    }
    private void Update()
    {
        if(Input.anyKeyDown == false)
        {
            return;
        }
        _btnList[_cursor].SetCursorOn(false);
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            _cursor = (_cursor + 1) % _length;
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            _cursor = (_cursor + _length - 1) % _length;
        }
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Select();
        }
        _btnList[_cursor].SetCursorOn(true);
    }
    void Select()
    {
        _btnList[_cursor].Execute();
    }
    private void OnDisable()
    {
        _retryButton.UnBindOnClickEvent(RetryGame);
        _giveupButton.UnBindOnClickEvent(GiveUp);
    }
    void RetryGame()
    {
        GameFlowManager.Inst.SetGamePhase(GamePhase.Ingame);
    }
    void GiveUp()
    {
        Application.Quit();
    }
}
