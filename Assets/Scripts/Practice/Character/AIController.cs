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

        List<Vector2Int> path = MapManager.Inst.GetPathToTarget(myPos, playerPos);
        if(path == null)
        {
            Debug.LogWarning("NULL path Error");
            return;
        }
        if (path.Count == 0) // There is no path to player
        {
            return;
        }
        Debug.LogWarning($"First path : {path[0]}");
        Vector2Int direction = path[0] - myPos;
        Vector2Int destPos = path[0];

        if (playerPos == destPos)
        {
            _controllerable.Attack(destPos);
            return;
        }

        if (MapManager.Inst.IsOccupied(destPos) == false && MapManager.Inst.IsWalkable(destPos))
        {
            _controllerable.Move(direction);
            return;
        } // Can move that dir

        // Cant move that dir, check adjacent direction
        var (leftDirection, rightDirection) = MyUtil.GetAdjacentDirections(direction);

        // left dir check
        destPos = myPos + leftDirection;
        if (MapManager.Inst.IsOccupied(destPos) == false && MapManager.Inst.IsWalkable(destPos))
        {
            _controllerable.Move(leftDirection);
            return;
        }

        // right dir check
        destPos = myPos + rightDirection;
        if (MapManager.Inst.IsOccupied(destPos) == false && MapManager.Inst.IsWalkable(destPos))
        {
            _controllerable.Move(rightDirection);
            return;
        }
    }
}
