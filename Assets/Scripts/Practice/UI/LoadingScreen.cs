using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public async UniTask StartLoading(GamePhase nextPhase, string[] loadList)
    {
        List<UniTask> loadingTasks = new List<UniTask>();
        foreach(var loadItem in loadList)
        {
            loadingTasks.Add(SafeLoadAsync(loadItem));
        }
        await UniTask.WhenAll(loadingTasks);
        GameFlowManager.Inst.SetGamePhase(nextPhase);
    }
    public async UniTask SafeLoadAsync(string path)
    {
        try
        {
            await PracticeResourceManager.Inst.LoadAssetAsync<GameObject>(path);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
}
