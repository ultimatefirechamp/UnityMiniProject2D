using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button _button;
    public Image _selectImage;
    public event Action OnEnterPointer;
    public event Action OnExitPointer;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnterPointer?.Invoke();
        _selectImage.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExitPointer?.Invoke();
        _selectImage.gameObject.SetActive(false);
    }
    public void BindOnClickEvent(Action callback)
    {
        _button.onClick.AddListener(new UnityEngine.Events.UnityAction(callback));
    }
    public void UnBindOnClickEvent(Action callback)
    {
        _button.onClick.RemoveListener(new UnityEngine.Events.UnityAction(callback));
    }
    public void BindOnPointerEnterEvent(Action callback)
    {
        OnEnterPointer += callback;
    }
    public void BindOnPointerExitEvent(Action callback)
    {
        OnExitPointer += callback;
    }
    public void UnBindOnPointerEnterEvent(Action callback)
    {
        OnEnterPointer -= callback;
    }
    public void UnBindOnPointerExitEvent(Action callback)
    {
        OnExitPointer -= callback;
    }

    public void UnBindAllEvent()
    {
        _button.onClick.RemoveAllListeners();
        OnEnterPointer = null;
        OnExitPointer = null;
    }
    public void SetCursorOn(bool isCursorOn)
    {
        _selectImage.gameObject.SetActive(isCursorOn);
    }
    public void Execute()
    {
        _button.onClick?.Invoke();
    }

}
