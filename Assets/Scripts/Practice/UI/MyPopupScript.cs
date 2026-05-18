using System.Collections;
using UnityEngine;

public class MyPopupScript : MonoBehaviour
{
    [SerializeField] private DaniTechUIButton _quitButton;
    [SerializeField] private DaniTechUIButton _selectOneButton;
    [SerializeField] private DaniTechUIButton _selectTwoButton;

    private void OnEnable()
    {
        _quitButton.BindOnClickButtonEvent(Quit);
        _selectOneButton.BindOnClickButtonEvent(SelectionOne);
        _selectTwoButton.BindOnClickButtonEvent(SelectionTwo);
        StartCoroutine(AutoClose());
    }
    private void OnDisable()
    {
        _quitButton.UnBindOnClickButtonEvent(Quit);
        _selectOneButton.UnBindOnClickButtonEvent(SelectionOne);
        _selectTwoButton.UnBindOnClickButtonEvent(SelectionTwo);
    }
    
    void Quit()
    {
        PracticeUIManager.Inst.CloseUIFromDic(UIType.SimplePopup);
        //Destroy(this.gameObject);
    }
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(3f);
        PracticeUIManager.Inst.CloseUIFromDic(UIType.SimplePopup);
    }
    void SelectionOne()
    {
        Debug.Log("You choose one selection");
    }
    void SelectionTwo()
    {
        Debug.Log("You choose second selection");
    }
}
