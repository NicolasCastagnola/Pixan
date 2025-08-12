using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] UnityEvent clickEvent;
    [SerializeField] Color normalColor ,hoverColor;
    [SerializeField] UnityEvent onHover;

    TextMeshProUGUI text;
    float fontSizeMax, fontSizeMaxHover;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        fontSizeMax = text.fontSizeMax;
        fontSizeMaxHover = text.fontSizeMax * 1.3f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
        text.fontSizeMax = fontSizeMaxHover;
        onHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
        text.fontSizeMax = fontSizeMax;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickEvent?.Invoke();
    }
}
