using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageSequenceElement : UploadFileElement, IPointerDownHandler, IBeginDragHandler, IEndDragHandler,IDragHandler
{
    [SerializeField] private Button alterarImg;
    public bool IsActive = false;

    private int index = -1;

    public int Index => index;
    public event Action<int> OnUploadFile;
    public event Action<ImageSequenceElement> OnDelete;

    public event Action<ImageSequenceElement, PointerEventData, bool> OnEndDragElement;

    private RectTransform rect;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    protected override void Start()
    {
        base.Start();
        alterarImg.onClick.AddListener(OpenFileDialogButtonOnClickHandler);
        showImage.gameObject.SetActive(false);
        alterarImg.gameObject.SetActive(false);
        fileData.gameObject.SetActive(false);
    }

    void Update()
    {

    }

    public void Init(int count, bool changeIndex = true)
    {
        IsActive = true;
        if(changeIndex)
            SetIndex(count);
        showImage.gameObject.SetActive(true);
        fileData.gameObject.SetActive(true);
        alterarImg.gameObject.SetActive(true);
    }

    public void IncreaseIndex()
    {
        if (IsActive)
        {
            SetIndex(index +1);
        }
    }
    
    public void DecreaseIndex()
    {
        if (IsActive)
        {
            SetIndex(index -1);
        }
    }
    
    public void SetIndex(int index)
    {
        this.index = index;
        if (index != -1)
        {
            fileData.text = (index + 1).ToString();
        }
        else
        {
            fileData.text = "";
        }
    }

    protected override void FilesWereOpenedEventHandler(File[] files)
    {
        base.FilesWereOpenedEventHandler(files);
        OnUploadFile?.Invoke(index);
    }

    protected override void CleanupButtonOnClickHandler()
    {
        base.CleanupButtonOnClickHandler();
        Delete();
    }
    
    private void Delete()
    {
        gameObject.SetActive(false);
        showImage.gameObject.SetActive(false);
        alterarImg.gameObject.SetActive(false);
        deleteFile.gameObject.SetActive(false);
        IsActive = false;
        OnDelete?.Invoke(this);
        SetIndex(-1);
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragElement?.Invoke(this, eventData, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsActive)
            rect.anchoredPosition += eventData.delta/canvas.scaleFactor;
    }
}