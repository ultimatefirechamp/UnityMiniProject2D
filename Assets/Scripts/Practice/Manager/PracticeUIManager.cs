using System.Collections.Generic;
using UnityEngine;

public class PracticeUIManager : MonoBehaviour
{
    [SerializeField] GameObject UIRoot;
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
    private void CreateUI(UIType type)
    {
        if(_createdUI.ContainsKey(type))
        {
            return;
        }
        GameObject uiPrefab = Instantiate(LoadUIObject(type), UIRoot.transform);
        _createdUI[type] = uiPrefab;
    }
    public GameObject GetCreatedUI(UIType type)
    {
        if( _createdUI.ContainsKey(type) == false)
        {
            CreateUI(type);
        }
        _createdUI[type].SetActive(true);
        return _createdUI[type];
    }
    public void CloseUIFromDic(UIType type)
    {
        CloseUI(type);
    }
    private void Start()
    {
        CreateUI(UIType.MainUI);
    }
}
