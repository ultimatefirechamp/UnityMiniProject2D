using UnityEngine;
using UnityEngine.UI;

public class DaniTechMobileMainUI : MonoBehaviour
{
    public Text Text_PlayerName;
    public Text Text_PlayerLevel;
    public DaniTechUIButton Button_StartCommand;

    private void OnEnable()
    {
        Text_PlayerName.text = "홍길동";
        Text_PlayerLevel.text = "Lv.3";
        Button_StartCommand.BindOnClickButtonEvent(OnClick_StartCommand);
    }

    public void OnClick_StartCommand()
    {
        // 이부분도 나중에 사라진다 -> 매니저가 애초에 생성될때 자동으로 해줄것임
        GameUtil.LoadFullData();


        var myHero = GameDataManager.Instance.GetCharacterData("character_hellena_01");

        if (myHero != null)
        {
            Text_PlayerName.text = myHero.Name;
            // 나중에 Description으로 캐릭터의 설명을 출력해봅시다
            // Text_PlayerLevel.text = myHero.D
        }
    }

}
