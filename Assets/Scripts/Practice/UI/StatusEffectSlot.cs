using UnityEngine;
using UnityEngine.UI;

public class StatusEffectSlot : MonoBehaviour
{
    [SerializeField] Image _spriteImg;
    [SerializeField] Text _stackCount;
    [SerializeField] Text _durationCount;

    public void RefreshSlot(StatusEffect effect)
    {
        _stackCount.text = $"{effect.Stack}";
        _durationCount.text = $"{effect.Duration}";
    }
    public void SetSlotSprite(Sprite sprite)
    {
        _spriteImg.sprite = sprite;
    }
}
