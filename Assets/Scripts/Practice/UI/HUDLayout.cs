using UnityEngine;

public class HUDLayout : MonoBehaviour
{
    [SerializeField] SliderTextHUD _hpSlider;
    [SerializeField] SliderTextHUD _spSlider;

    void SetHpBar(int maxHp, int hp)
    {
        _hpSlider.SetSliderRatio(hp, maxHp);
        _hpSlider.SetText($"{hp} / {maxHp}");
    }
    void SetSpBar(int maxSp, int sp)
    {
        _spSlider.SetSliderRatio(sp, maxSp);
        _spSlider.SetText($"{sp} / {maxSp}");
    }

    public void RegistPlayer(CharacterScript player)
    {
        player.OnDamaged += SetHpBar;
        player.OnSpChanged += SetSpBar;
    }
    public void UnRegistPlayer(CharacterScript player)
    {
        player.OnDamaged -= SetHpBar;
        player.OnSpChanged -= SetSpBar;
    }
}
