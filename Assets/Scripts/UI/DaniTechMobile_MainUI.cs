using UnityEngine;
using UnityEngine.UI;

public class DaniTechMobile_MainUI : MonoBehaviour
{
    // 1) 이 유아이에서 뭐가 필요한가?
        // 코드에서 어떤 컴포넌트를 제어할 것인가를 여기서 명시 
        // Text나 이미지를 부가적으로 수정하려고 하는 경우도 다 여기서 참조 하세요
    [SerializeField] private DaniTechUIButton Btn_MyProfile;
    [SerializeField] private DaniTechUIButton Btn_Option;
    [SerializeField] private DaniTechUIButton Btn_StartBattle;
    [SerializeField] private DaniTechUIButton Btn_OpenInfoBook;
    [SerializeField] private DaniTechUIButton Btn_Inventory;

    [SerializeField] private Text Text_CurrentStageNumer;
    // +이미지도 바꿀 수 있다
    //  [SerializeField] private Image Image_AAA;

    private void OnEnable()
    {
        Btn_MyProfile.BindOnClickButtonEvent(OnClick_OpenMyProfile);
        Btn_Option.BindOnClickButtonEvent(OnClick_OpenOption);
        Btn_StartBattle.BindOnClickButtonEvent(OnClick_StartBattle);
        Btn_OpenInfoBook.BindOnClickButtonEvent(OnClick_OpenInfoBook);
        Btn_Inventory.BindOnClickButtonEvent(OnClick_OpenInventory);
    }

    public void OnClick_OpenMyProfile()
    {
        GameDataTester.StartDataTest();
        Debug.Log("프로필이 열렸다");
    }

    public void OnClick_OpenOption()
    {
        Debug.Log("설정이 열렸다");
    }

    public void OnClick_StartBattle()
    {
        // 전투시작이 눌려지면
        // 결국엔 배틀매니저, 맵 매니저, 유아이매니저, 스테이지매니저
        // 데이터 매니저 이런 애들한테 연락 다 돌려서 전투 장면을 구성하는 겁니다
        // 로딩이미지 먼저 뜨고 -> 그 뒤에서 모든 것을 다 준비하고 다되면 로딩이미지 끔
        Debug.Log("전투 시작");

    }

    public void OnClick_OpenInfoBook() 
    {
        Debug.Log("팝업이 열렸다");
        // 팝업의 참조를 여기 멤버변수에서 열고
        // 그 멤버변수를 씬에서 등록 (팝업의 오브젝트를 미리 등록)
        // 그 팝업의 gameobject.SetActive(true) / false로 활성/비활성화 제어 가능!

    }

    public void OnClick_OpenInventory()
    {
        Debug.Log("인벤토리");

    }






}
