using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiZoomableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button _button;
    [SerializeField] private float _defaultScale = 1f;
    [SerializeField] private float _onHoverScale = 1.1f;
    [SerializeField] private float _pressedScale = 0.9f;

    private bool _pressed = false;

    private void Reset()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        SetButtonScale(_defaultScale);
    }

    private void SetButtonScale(float scale)
    {
        _button.transform.localScale = new Vector3(scale, scale);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            SetButtonScale(_onHoverScale);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            SetButtonScale(_defaultScale);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_button.interactable && eventData.button == PointerEventData.InputButton.Left)
        {
            SetButtonScale(_pressedScale);
            _pressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_button.interactable && eventData.button == PointerEventData.InputButton.Left && _pressed)
        {
            SetButtonScale(_onHoverScale);
            _pressed = false;
        }
    }
}