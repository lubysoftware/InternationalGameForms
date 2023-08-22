using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageSequenceElement : ImageElement, IEndDragHandler,IDragHandler
{
    public event Action<ImageElement, PointerEventData, bool> OnEndDragElement;

    private RectTransform rect;
    [SerializeField] private Canvas canvas;
    
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(IsActive)
            OnEndDragElement?.Invoke(this, eventData, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsActive)
            rect.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }
}