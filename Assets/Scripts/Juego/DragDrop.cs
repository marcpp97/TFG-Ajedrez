using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler
{
    [SerializeField]
    private Canvas canvas;

    public RectTransform ventana;

    public void OnDrag(PointerEventData eventData)
    {
        ventana.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
