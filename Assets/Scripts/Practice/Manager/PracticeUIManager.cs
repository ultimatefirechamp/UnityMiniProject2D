using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public enum UILayerType
{
    NONE,
    Background,
    HUD,
    Middle,
    PopUp,
    Front
}
public enum UIType
{
    SimplePopup,
    MainUI,
    HUD,
    HPBarGroup,
    StatusEffectLayer,
    StartScreen,
    LoadingScreen,
    GameoverScreen
}

public class PracticeUIManager : MonoBehaviour
{
    [SerializeField] GameObject UIRoot;
    [SerializeField] Transform PopupRoot;
    [SerializeField] Transform FrontRoot;
    [SerializeField] Transform MiddleRoot;
    [SerializeField] Transform HudRoot;

    private static PracticeUIManager _instance;
    public static PracticeUIManager Inst { get { return _instance; } }
    private Dictionary<UIType, GameObject> _createdUI;
    private HashSet<UIType> _openedUI;
    private string GetUIPath(UIType type)
    {
        string path = string.Empty;
        switch (type)
        {
            case UIType.HPBarGroup:
                path = "Prefabs/Practice_KCK/UI/HPBar_Group";
                break;
            case UIType.MainUI:
                path = "Prefabs/Practice_KCK/UI/MainUI";
                break;
            case UIType.SimplePopup:
                path = "Prefabs/Practice_KCK/UI/MyPopup";
                break;
            case UIType.HUD:
                path = "Prefabs/Practice_KCK/UI/HUD_Layout";
                break;
            case UIType.StatusEffectLayer:
                path = "Prefabs/Practice_KCK/UI/StatusEffectPanel";
                break;
            case UIType.LoadingScreen:
                path = "Prefabs/Practice_KCK/UI/LoadingScreen";
                break;
            case UIType.StartScreen:
                path = "Prefabs/Practice_KCK/UI/LobbyScreen";
                break;
            case UIType.GameoverScreen:
                path = "Prefabs/Practice_KCK/UI/GameoverScreen";
                break;
        }
        return path;
    }
    private GameObject LoadUIObject(UIType type)
    {
        string path = GetUIPath(type);
        Debug.Log(path);
        GameObject uiPrefab = (GameObject)Resources.Load(path);
        return uiPrefab;
    }
    private void OpenUI(UIType type)
    {
        if(_createdUI.ContainsKey(type) == false)
        {
            return;
        }
        _createdUI[type].SetActive(true);
        _openedUI.Add(type);
    }
    private void CloseUI(UIType type)
    {
        if (_createdUI.ContainsKey(type) == false)
        {
            Debug.LogWarning($"No such UI {type}");
            return;
        }
        _createdUI[type].SetActive(false);
        _openedUI.Remove(type);
    }
    private void Awake()
    {
        _instance = this;
        _createdUI = new Dictionary<UIType, GameObject>();
        _openedUI = new HashSet<UIType>();
    }
    public void CreateUI(UIType type, UILayerType layer = UILayerType.NONE)
    {
        Transform root = MiddleRoot;
        if(_createdUI.ContainsKey(type))
        {
            return;
        }
        if(layer != UILayerType.NONE)
        {
            switch (layer)
            {
                case UILayerType.Front:
                    root = FrontRoot;
                    break;
                case UILayerType.HUD:
                    root = HudRoot;
                    break;
                case UILayerType.Middle:
                    root = MiddleRoot;
                    break;
                case UILayerType.PopUp:
                    root = PopupRoot;
                    break;
            }
        }
        GameObject uiPrefab = Instantiate(LoadUIObject(type), root);
        _createdUI[type] = uiPrefab;
        uiPrefab.SetActive(false);
    }
    public GameObject GetCreatedUI(UIType type, UILayerType layer = UILayerType.NONE)
    {
        if( _createdUI.ContainsKey(type) == false)
        {
            CreateUI(type, layer);
        }
        _createdUI[type].SetActive(true);
        _openedUI.Add(type);
        return _createdUI[type];
    }
    public void CloseUIFromDic(UIType type)
    {
        CloseUI(type);
    }
    public void CloseAll()
    {
        foreach(var ui in _openedUI)
        {
            _createdUI[ui].SetActive(false);
        }
        _openedUI.Clear();
    }
    public void ClearUIFromDic(UIType type)
    {
        if(_openedUI.Contains(type))
        {
            _openedUI.Remove(type);
        }
        if(_createdUI.TryGetValue(type, out var ui))
        {
            Destroy(ui.gameObject);
            _createdUI.Remove(type);
        }
    }
    public void ClearAllDic()
    {
        foreach(var uiKV in _createdUI)
        {
            if(_openedUI.Contains(uiKV.Key))
            {
                _openedUI.Remove(uiKV.Key);
            }
            Destroy(uiKV.Value.gameObject);
        }
        _createdUI.Clear();
    }
    public async UniTask LoadUIAsync(string path)
    {
        // 음... 글쎄... 이거는 그냥 ResourceManager에서 처리하는게 더 나을거 같은데..?
        // 먼저 Instantiate하는게 아닐거라면..?
        // 그냥 데이터만 먼저 로드 시켜놓고 싶으면 ResourceManager가 나을 듯... 지우자 이거는.
        try
        {
            GameObject loadedUIObject = await PracticeResourceManager.Inst.LoadObjectAsync(path);
            if (loadedUIObject == null)
            {
                Debug.LogWarning("Object Is not Exist");
                return;
            }
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    private void Start()
    {
        //CreateUI(UIType.MainUI);
    }
}
