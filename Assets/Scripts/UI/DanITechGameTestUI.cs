using UnityEngine;

public class DanITechGameTestUI : MonoBehaviour
{
    public void OnClick_DataLoadTest()
    {
        GameDataTester.StartDataTest();
    }
   
    public void OnClick_SelectTestBtn()
    {
        DaniTechGameObjectManager.Inst.RequestSpawnEnemy();
    }
}
