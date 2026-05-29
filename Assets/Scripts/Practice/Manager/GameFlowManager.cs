using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    None,
    Lobby,
    Loading,
    Ingame,
    GameOver,
    Clear
}

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Inst { get; private set; }
    GamePhase _currentPhase = GamePhase.None;
    PrefabScene _currentScene;
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        PracticeUIManager.Inst.CreateUI(UIType.StartScreen, UILayerType.Front);
        PracticeUIManager.Inst.CreateUI(UIType.LoadingScreen, UILayerType.Front);
        SetGamePhase(GamePhase.Lobby);
    }
    public void SetGamePhase(GamePhase nextPhase)
    {
        if(nextPhase == _currentPhase) return;
        if (_currentScene != null)
        {
            _currentScene.CloseScene();
        }
        PracticeUIManager.Inst.CloseAll();
        _currentPhase = nextPhase;
        switch(_currentPhase)
        {
            case GamePhase.None:
                break;
            case GamePhase.Lobby:
                PracticeUIManager.Inst.GetCreatedUI(UIType.StartScreen);
                break;
            case GamePhase.Loading:
                GameObject uiscreen = PracticeUIManager.Inst.GetCreatedUI(UIType.LoadingScreen);
                string[] loadList = { "Assets/AddressableAsset/IngameScene.prefab" };
                uiscreen.GetComponent<LoadingScreen>().StartLoading(GamePhase.Ingame, loadList).Forget();
                break;
            case GamePhase.Ingame:
                GameObject ingameScene;
                string path = "Assets/AddressableAsset/IngameScene.prefab";
                if (PracticeResourceManager.Inst.IsContainAsset(path))
                {
                    ingameScene = PracticeResourceManager.Inst.GetAssetWithoutLoad<GameObject>(path);
                    _currentScene = Instantiate(ingameScene).GetComponent<IngameScene>();
                    _currentScene.gameObject.SetActive(true);
                }
                else
                {
                    SetGamePhase(GamePhase.Loading);
                }
                break;
            case GamePhase.GameOver:
                MapManager.Inst.ResetManager();
                BattleManager.Inst.ResetManager();
                PracticeUIManager.Inst.GetCreatedUI(UIType.GameoverScreen);
                break;
            case GamePhase.Clear:
                break;
        }
    }
}
