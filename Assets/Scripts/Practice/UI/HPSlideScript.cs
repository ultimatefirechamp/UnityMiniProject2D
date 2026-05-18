using UnityEngine;
using UnityEngine.UI;

public class HPSlideScript : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    public void SetHpSlideRatio(int max, int current)
    {
        hpSlider.value = (float) current / max;
    }
    public void MoveSliderPos(Transform transform)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (IsInScreen(screenPos) == false)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.transform.position = screenPos;
            this.gameObject.SetActive(true);
        }
    }
    //HUD UI를 통해서 ID와 position을 전달하는 식으로.. Monster와 일치하는걸 찾아서 하자

    public bool IsInScreen(Vector2 screenPos)
    {
        if (screenPos.x < 0 || screenPos.x > Camera.main.pixelWidth ||
            screenPos.y < 0 || screenPos.y > Camera.main.pixelHeight)
        {
            return false;
        }
        return true;
    }
}
