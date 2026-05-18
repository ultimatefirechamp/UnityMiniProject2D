using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Timeline;

public class AIController : MonoBehaviour
{
    IControllable _controllerable;
    [SerializeField] Transform playerTransform;

    void FindControllableComponent()
    {
        if (this.gameObject.TryGetComponent<IControllable>(out var controllerable))
        {
            _controllerable = controllerable;
            return;
        }
        Debug.LogWarning($"{this.gameObject.name} : Can't Find Controllable Component.");
    }
    private void Start()
    {
        FindControllableComponent();
        BattleManager.Inst.RegistEnemy(_controllerable as CharacterScript);
    }

    public void OnTurn()
    {
        Vector2Int myPos = MapManager.Inst.WorldToArrayPos(transform.position);
        Vector2Int playerPos = MapManager.Inst.WorldToArrayPos(playerTransform.position);
        Debug.Log($"PlayerPos : {playerPos} | AIPos : {myPos}");

        List<Vector2Int> path = MapManager.Inst.GetPathToTarget(myPos, playerPos);
        if(path == null)
        {
            Debug.LogWarning("NULL path Error");
            return;
        }
        Debug.LogWarning($"First path : {path[0]}");

        if (path.Count > 0)
        {

            Vector2Int direction = path[0] - myPos;
            Vector2Int destPos = myPos + direction;
            if(MapManager.Inst.WorldToArrayPos((Vector2)playerPos) == destPos)
            {
                _controllerable.Attack(destPos);
                return;
            }
            _controllerable.Move(direction);
        }
    }

    public void Update()
    { }
}
