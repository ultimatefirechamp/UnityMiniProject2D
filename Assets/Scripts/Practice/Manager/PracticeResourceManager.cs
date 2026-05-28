using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PracticeResourceManager : MonoBehaviour
{
    public static PracticeResourceManager Inst { get; set; }

    private void Awake()
    {
        Inst = this;
    }

    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();
 
    public Sprite LoadSprite(string path)
    {
        Sprite loadedSprite = Resources.Load<Sprite>(path);
        if (loadedSprite != null)
        {
            return loadedSprite;
        }
        Debug.LogWarning($"{path} : Can't load Sprite");
        return null;
    }
    public GameObject LoadPrefab(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if(prefab != null)
        {
            return prefab;
        }
        Debug.LogWarning($"{path} : Can't load Object");
        return null;
    }
    public void AddressableLoadSprite_Callback(string path, Action<Sprite> callback)
    {
        if (_handles.TryGetValue(path, out AsyncOperationHandle handle))
        {
            callback?.Invoke(handle.Result as Sprite);
            return;
        }
        AsyncOperationHandle<Sprite> loadedHandle = Addressables.LoadAssetAsync<Sprite>(path);
        void lambda(AsyncOperationHandle<Sprite> op)
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[path] = op;
                callback?.Invoke(op.Result as Sprite); // 로드된 데이터를 sprite에 적용하는 코드?명령을 Invoke시키겠다.
            }
            else
            {
                Debug.LogWarning($"Fail to load sprite data {path}");
            }
        }
        loadedHandle.Completed += lambda;
        loadedHandle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[path] = op; 
                callback?.Invoke(op.Result as Sprite);
            }
            else
            {
                Debug.LogWarning($"Fail to load sprite data {path}");
            }
        };
    }
    public async UniTask<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object
    {
        if(_handles.ContainsKey(path))
        {
            return _handles[path].Result as T;
        }
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(path);
        try
        {
            await handle.ToUniTask();
            _handles.Add(path, handle);
            return handle.Result;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return null;
    }
    public async UniTask<GameObject> LoadObjectAsync(string path)
    {
        if(_handles.ContainsKey(path))
        {
            return _handles[path].Result as GameObject;
        }
        AsyncOperationHandle handle = Addressables.LoadAssetAsync<GameObject>(path);
        try
        {
            await handle.ToUniTask();
            _handles[path] = handle;
            return handle.Result as GameObject;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return null;
    }
    
    public void Unload(string path)
    {
        if (_handles.ContainsKey(path) == false)
        {
            return;
        }
        Addressables.Release(_handles[path]);
        _handles.Remove(path);
    }

    public void ClearAllHandle()
    {
        foreach (var handleKV in _handles)
        {
            Addressables.Release(handleKV.Value);
        }
        _handles.Clear();
    }
}
