using UnityEngine;

public class StatusUI : MonoBehaviour
{

    public void RegisterOwner(CharacterScript _onwer)
    {

    }

    void MoveUI(Transform transform)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        rectTransform.position = screenPos;
    }

}
