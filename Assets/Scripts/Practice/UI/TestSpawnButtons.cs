using System;
using UnityEngine;
using UnityEngine.UI;

public class TestSpawnButtons : MonoBehaviour
{
    [SerializeField] private Button SpawnBtn;
    [SerializeField] private Button OpenPopUpBtn;
    [SerializeField] private InputField IDInputField;

    private void Start()
    {
        
    }
    private void OnEnable()
    {
        SpawnBtn.onClick.AddListener(new UnityEngine.Events.UnityAction(SpawnEnemy));
        OpenPopUpBtn.onClick.AddListener(new UnityEngine.Events.UnityAction(OpenPopUp));
        IDInputField.onSubmit.AddListener(new UnityEngine.Events.UnityAction<string>(SpawnEnemyFromID));
    }
    private void OnDisable()
    {
        SpawnBtn.onClick.RemoveAllListeners();
        OpenPopUpBtn.onClick.RemoveAllListeners();
        IDInputField.onSubmit.RemoveAllListeners();
    }
    public void OpenPopUp()
    {
        PracticeUIManager.Inst.GetCreatedUI(UIType.SimplePopup);
    }
    public void SpawnEnemy()
    {
        BattleManager.Inst.SpawnEnemy();
    }
    public void SpawnEnemyFromID(string id)
    {
        MonsterData data = GameDataManager.Instance.GetMonsterData(id);
        if (data == null)
        {
            Debug.LogWarning($"{id} is not loaded To monsterData");
            return;
        }
        CharacterScript spawnedEnemy = BattleManager.Inst.GetSpawnEnemy();
        spawnedEnemy.SetCharacter(data);
        Debug.Log($"MONSTER SPAWNED {id}\n{data}");
    }
}
