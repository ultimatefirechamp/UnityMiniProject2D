using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Inst { get { return _instance; } }
    static ObjectManager _instance;
    [SerializeField] GameObject unitTemplate;

    List<CharacterScript> _SpawnedUnit;

    private void Awake()
    {
        _instance = this;
        _SpawnedUnit = new List<CharacterScript>();
    }

    private void Start()
    {
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartGame().Forget();
        }
    }
    public async UniTask StartGame()
    {
        GameObject ingameScene = await PracticeResourceManager.Inst.LoadAssetAsync<GameObject>("Assets/AddressableAsset/IngameScene.prefab");
        GameObject createdScene = Instantiate(ingameScene);

        createdScene.SetActive(true);
    }

    public GameObject SpawnUnit(Vector2Int position, Transform root = null)
    {
        GameObject playerObject = Instantiate(unitTemplate);
        playerObject.GetComponent<CharacterScript>().SetGridPosition(position);
        MapManager.Inst.OccupyTile(position, playerObject.GetComponent<CharacterScript>());
        _SpawnedUnit.Add(playerObject.GetComponent<CharacterScript>());
        return playerObject;
    }

}
