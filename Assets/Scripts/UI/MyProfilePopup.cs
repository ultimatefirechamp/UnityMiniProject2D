using UnityEngine;
using UnityEngine.UI;

public class MyProfilePopup : MonoBehaviour
{
    [SerializeField] private Text Text_Title;
    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_Description;

    public void Start()
    {
        GameUtil.LoadFullData();

        var myHero = GameDataManager.Instance.GetCharacterData("character_hellena_01");

        if (myHero != null)
        {
            Debug.Log($"로드된 캐릭터 이름: {myHero.Name}");
        }

        Text_Name.text = myHero.Name;
        Text_Title.text = myHero.Name;

        string dummyDescription = string.Empty;
        // 스킬 정보가 있다면
        if (myHero.SkillList != string.Empty)
        {
            string[] skillNameList = myHero.SkillList.Split(',');
            foreach (string skillName in skillNameList)
            {
                var skillData = GameDataManager.Instance.GetSkill(skillName);
                if (skillData != null)
                {
                    dummyDescription += $"로드된 캐릭터: {myHero.Name}는 {skillData.Name}을 갖고 있다!";
                }
            }
        }

        Text_Description.text = dummyDescription;

        //if (string.IsNullOrEmpty(myHero.UseWeaponId) == false)
        //{
        //    var weaponData = GameDataManager.Instance.GetWeaponData(myHero.UseWeaponId);
        //    if (weaponData != null)
        //    {
        //        Debug.Log($"로드된 캐릭터: {myHero.Name}는 사용무기로 {weaponData.Name}을 갖고 있다!");
        //    }
        //}
    }
}
