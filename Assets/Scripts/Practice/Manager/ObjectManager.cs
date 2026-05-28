using System.Collections.Generic;
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
        //SpawnPlayer(new Vector2Int(0, 0));
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
