using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaleteCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _activeImg;
    public event Action<PaleteCell> OnCellClick;
    public Color color;

    public void Setup(Color color) 
    {
        GetComponent<Image>().color = color;
        this.color = color;
    }
    public void SetActive(bool isActive) 
    {
        _activeImg.enabled = isActive;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCellClick?.Invoke(this);
    }
}
