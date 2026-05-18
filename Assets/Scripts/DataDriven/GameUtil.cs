using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameUtil
{
    public static void LoadFullData()
    {
        // 게임 로딩할 때 불러올 데이터는 여기서! 
        GameDataManager.Instance.LoadSkillData(GetFullDataPath("Skill"));
        GameDataManager.Instance.LoadCharacterData(GetFullDataPath("Character"));
        GameDataManager.Instance.LoadWeaponData(GetFullDataPath("Weapon"));
        GameDataManager.Instance.LoadCostumeData(GetFullDataPath("Costume"));
    }

    public static Sprite LoadSpriteCanBeNull(string spritePath)
    {
        Sprite loadedData = Resources.Load<Sprite>(spritePath);
        if(loadedData == null)
        {
            return null;
        }
        return loadedData;
    }

    public static string GetFullDataPath(string dataTableName)
    {
        if (string.IsNullOrEmpty(dataTableName))
        {
            Debug.Log("테이블 이름이 올바르지 않습니다!");
            return string.Empty;
        }

        // string path = @"C:예시-여러분의 상위 폴더\ComputerBasics_DaniTech_Work\JsonConverter\JsonOutput\Character.json";
        // 제이슨 컨버터를 한번 실행해서 JsonCoutput에 Character.json을 미리 만들어뒀는지도 꼭 확인해주세요!
        // 상대경로 ../../ 추후 툴 이름이나 경로가 변경되면 여길 확인하세요!
        string relativePath = $"JsonConverter/JsonOutput/{dataTableName}.json";
        string fullPath = Path.GetFullPath(relativePath);
        return fullPath;
    }


    public static int CalcCharacterFinalDamage(int curCharacterLevel, int levelPerDamage, bool isCritical)
    {
        int damagePerLevel = (curCharacterLevel + levelPerDamage);
        int finalDamage = isCritical ? (damagePerLevel * 2) : damagePerLevel;
        return finalDamage;
    }
}