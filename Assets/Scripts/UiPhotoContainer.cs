using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiPhotoContainer : MonoBehaviour
{
    [SerializeField] private Image _photoImage;
    [SerializeField] private TextMeshProUGUI _description;

    public void Init(Sprite sprite, string description)
    {
        _photoImage.sprite = sprite;

        if (!string.IsNullOrEmpty(description))
        {
            _description.text = description;
        }
    }
}