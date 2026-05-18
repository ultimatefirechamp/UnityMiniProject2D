using UnityEngine;
using UnityEngine.UI;

public class TestSpawnButtons : MonoBehaviour
{
    [SerializeField] private Button SpawnBtn;
    [SerializeField] private Button OpenPopUpBtn;

    private void Start()
    {
        
    }
    private void OnEnable()
    {
        SpawnBtn.onClick.AddListener(new UnityEngine.Events.UnityAction(SpawnEnemy));
        OpenPopUpBtn.onClick.AddListener(new UnityEngine.Events.UnityAction(OpenPopUp));
    }
    private void OnDisable()
    {
        SpawnBtn.onClick.RemoveAllListeners();
        OpenPopUpBtn.onClick.RemoveAllListeners();
    }
    public void OpenPopUp()
    {
        PracticeUIManager.Inst.GetCreatedUI(UIType.SimplePopup);
    }
    public void SpawnEnemy()
    {
        BattleManager.Inst.SpawnEnemy();
    }
}
