using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;


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
        //PracticeUIManager.Inst.GetCreatedUI(UIType.StartScreen);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //StartGame().Forget();
        }
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
