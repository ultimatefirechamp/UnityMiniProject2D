using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderTextHUD : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;

    public void SetSliderRatio(int current, int max)
    {
        _slider.value = (float) current / (float) max;
    }

    public void SetText(string desc)
    {
        _text.text = desc;
    }
}
