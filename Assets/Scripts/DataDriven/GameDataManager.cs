using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
        GameUtil.LoadFullData();
    }

    // --- JsonUtility의 한계를 극복하기 위한 Wrapper 클래스 ---
    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items; // JSON 파일의 루트 키 이름이 "items"여야 함
    }
    // ---------------------------------------------------

    public Dictionary<string, CharacterData> CharacterDataList { get; private set; } = new Dictionary<string, CharacterData>();
    public Dictionary<string, SkillData> SkillDataList { get; private set; } = new Dictionary<string, SkillData>();
    public Dictionary<string, SkillRecord> SkillRecordList { get; private set; } = new Dictionary<string, SkillRecord>();
    public Dictionary<string, WeaponData> WeaponDataList { get; private set; } = new Dictionary<string, WeaponData>();
    public Dictionary<string, CostumeData> CostumeDataList { get; private set; } = new Dictionary<string, CostumeData>();
    public Dictionary<string, MonsterData> MonsterDataList { get; private set; } = new Dictionary<string, MonsterData>();

    private Dictionary<string, T> LoadData<T>(string tableName) where T : GameDataBase
    {
        // 1. 경로 설정 (확장자 .json 제외!)
        // Resources/JsonOutput 폴더
        string resourcePath = $"JsonOutput/{tableName}";

        // 2. 리소스 로드
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        // 3. 파일 존재 여부 체크
        if (textAsset == null)
        {
            Debug.LogError($"[Error] 리소스를 찾을 수 없습니다: Resources/{resourcePath}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            // 4. JsonUtility용 Wrapper 트릭 적용
            string wrappedJson = "{\"items\":" + jsonString + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");
                // ToDictionary를 사용하려면 각 클래스(T)에 Id 필드가 있어야 합니다.
                return wrapper.items.ToDictionary(item => item.Id.ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }

    public void LoadSkillData(string jsonPath)
    {
        SkillDataList = LoadData<SkillData>(jsonPath);
        foreach(var skillData in SkillDataList)
        {
            SkillRecord record = new SkillRecord(skillData.Value);
            SkillRecordList.Add(skillData.Key, record);
        }
    }

    public void LoadCharacterData(string jsonPath)
    {
        CharacterDataList = LoadData<CharacterData>(jsonPath);
    }

    public void LoadWeaponData(string jsonPath)
    {
        WeaponDataList = LoadData<WeaponData>(jsonPath);
    }

    public void LoadCostumeData(string jsonPath)
    {
        CostumeDataList = LoadData<CostumeData>(jsonPath);
    }
    public void LoadMonsterData(string jsonPath)
    {
        MonsterDataList = LoadData<MonsterData>(jsonPath);
    }


// [아래는 사용을 위한 부분들을 메서드 정의] =========================================================================================
// Get과 Find이름을 꼭 구별 하자!

    public CharacterData GetCharacterData(string id)
    {
        if (CharacterDataList == null || string.IsNullOrEmpty(id)) return null;

        return CharacterDataList.TryGetValue(id, out var item) ? item : null;
    }

    public SkillData GetSkill(string id)
    {
        if (SkillDataList == null || string.IsNullOrEmpty(id)) return null;

        return SkillDataList.TryGetValue(id, out var item) ? item : null;
    }
    public SkillRecord GetSkillRecord(string id)
    {
        if (SkillRecordList == null || string.IsNullOrEmpty(id)) return null;

        return SkillRecordList.TryGetValue(id, out var item) ? item : null;
    }

    public WeaponData GetWeaponData(string id)
    {
        if (WeaponDataList == null || string.IsNullOrEmpty(id)) return null;

        return WeaponDataList.TryGetValue(id, out var data) ? data : null;
    }

    public CostumeData GetCostumeData(string id)
    {
        if (CostumeDataList == null || string.IsNullOrEmpty(id)) return null;

        return CostumeDataList.TryGetValue(id, out var data) ? data : null;
    }

    public MonsterData GetMonsterData(string id)
    {
        if(MonsterDataList == null || string.IsNullOrEmpty(id)) return null;
        return MonsterDataList.TryGetValue(id,out var data) ? data : null;
    }
}