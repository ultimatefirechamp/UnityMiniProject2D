using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    
    public Sprite AddressableLoadSprite_Value(string path)
    {
        if (_handles.TryGetValue(path, out AsyncOperationHandle handle))
        {
            return handle.Result as Sprite;
        }
        //AsyncAwait 개념 선행.
        AsyncOperationHandle<Sprite> loadedHandle = Addressables.LoadAssetAsync<Sprite>(path);
        return loadedHandle.Result as Sprite; // 복불복... 될수도있고 안될수도 있습니다.
        // 이런식으로는 불가능하려나... 비동기인 이상 이 함수가 sprite가 언제 로드될지를 모르기 때문에 직접 return하는건 좀 힘들 거 같은데...
        // LoadAssetAsync가 언제 완료 될 지 모름. return을 달라고 하는 시점에서는 로드가 안되서 null값이 나올것 같음.
        // 강사님 면담. 유니태스크, AsyncAwait도 한번 공부해봐라.
    }
}
