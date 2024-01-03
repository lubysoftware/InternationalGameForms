using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ImageElement : UploadFileElement
{
    public bool IsActive = false;
    [SerializeField] private bool managedByParent = true;
    public event Action<bool> OnUploadFile;
    public event Action<int, bool> OnUploadFileIndex;
    public event Action<ImageElement> OnDelete;

    public int Index => transform.GetComponentInParent<ImageFrame>().Index;
    

    public void AddImage()
    {
        OpenFileDialogButtonOnClickHandler();
    }

    public void Init()
    {
        IsActive = true;
        showImage.gameObject.SetActive(true);
        transform.GetComponentInParent<ImageFrame>().SetActiveState(true);
    }

    protected override void FilesWereOpenedEventHandler(File[] files)
    {
        base.FilesWereOpenedEventHandler(files);
        OnUploadFile?.Invoke(IsActive);
        OnUploadFileIndex?.Invoke(Index,IsActive);
        if(!managedByParent)
            Init();
    }

    protected override void CleanupButtonOnClickHandler()
    {
        base.CleanupButtonOnClickHandler();
        transform.GetComponentInParent<ImageFrame>().SetActiveState(false);
        Delete();
    }
    
    public void Delete()
    {
        showImage.sprite = previewImage;
        IsActive = false;
        IsFilled = false;
        OnDelete?.Invoke(this);
    }
    
    public override void FinishedDownloadFileData(Texture2D texture)
    {
        base.FinishedDownloadFileData(texture);
        if(!managedByParent)
            Init();
    }

    public bool IsEnabled()
    {
        return isActiveAndEnabled;
    }
}
