using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageSequenceElement : UploadFileElement, IEndDragHandler,IDragHandler
{
    public bool IsActive = false;
    
    public event Action<bool> OnUploadFile;
    public event Action<ImageSequenceElement> OnDelete;

    public event Action<ImageSequenceElement, PointerEventData, bool> OnEndDragElement;

    private RectTransform rect;
    [SerializeField] private Canvas canvas;
    
    public int Index => transform.GetComponentInParent<ImageSequenceFrame>().Index;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void AddImage()
    {
        OpenFileDialogButtonOnClickHandler();
    }

    public void Init(int count, bool changeIndex = true)
    {
        IsActive = true;
        showImage.gameObject.SetActive(true);
        transform.GetComponentInParent<ImageSequenceFrame>().SetActiveState(true);
    }

    protected override void FilesWereOpenedEventHandler(File[] files)
    {
        base.FilesWereOpenedEventHandler(files);
        OnUploadFile?.Invoke(IsActive);
    }

    protected override void CleanupButtonOnClickHandler()
    {
        base.CleanupButtonOnClickHandler();
        transform.GetComponentInParent<ImageSequenceFrame>().SetActiveState(false);
        Delete();
    }
    
    public void Delete()
    {
        showImage.gameObject.SetActive(false);
        IsActive = false;
        OnDelete?.Invoke(this);
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