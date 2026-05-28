using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class SelectionLayout : MonoBehaviour
{
    [SerializeField] private ButtonBase startButton;
    [SerializeField] private ButtonBase quitButton;
    List<ButtonBase> buttonList;
    int cursor = 0;
    int optionSize = 2;
    private void OnEnable()
    {
        //startButton.BindOnClickEvent(StartGame);
        //quitButton.BindOnClickEvent(EndGame);
        buttonList = new List<ButtonBase>();
        buttonList.Add(startButton);
        buttonList.Add(quitButton);
    }

    private void Update()
    {
        if(Input.anyKeyDown == false)
        {
            return;
        }
        buttonList[cursor].SetCursorOn(false);
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cursor = (cursor + 3) % optionSize;
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            cursor = (cursor + 1) % optionSize;
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Select();
        }
        buttonList[cursor].SetCursorOn(true);
    }
    public void BindStartButtonEvent(Action callback)
    {
        startButton.BindOnClickEvent(callback);
    }
    public void BindQuitButtonEvent(Action callback)
    {
        quitButton.BindOnClickEvent(callback);
    }
    public void UnBindStartButtonEvent(Action callback)
    {
        startButton.UnBindOnClickEvent(callback);
    }
    public void UnBindQuitButtonEvent(Action callback)
    {
        quitButton.UnBindOnClickEvent(callback);
    }
    void Select()
    {
        buttonList[cursor].Execute();
    }
    void StartGame()
    {
        Debug.Log("Game Start");
    }
    void EndGame()
    {
        Debug.Log("Game Ended");
    }
}
