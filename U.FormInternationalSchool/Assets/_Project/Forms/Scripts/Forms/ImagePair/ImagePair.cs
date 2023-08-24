using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class ImagePair : MonoBehaviour
{
    [SerializeField] private ImageFrame[] frames;
    public char Id;

    private void Start()
    {
        foreach (var frame in frames)
        {
            frame.Image.OnUploadFileIndex += OnAddImage;
        }
    }
    public bool IsCompleted()
    {
        foreach (var frame in frames)
        {
            Debug.LogError(Id +" is active " + frame.IsActive);
            Debug.LogError(Id +" uploaded file null " + frame.Image.UploadedFile == null);
            Debug.LogError(Id+ " is filled " + frame.Image.IsFilled);
            if (!frame.IsActive || (frame.Image.UploadedFile == null && !frame.Image.IsFilled))
            {
                Debug.LogError("false");
                return false;
            }
        }

        return true;
    }

    public List<File> GetFiles()
    {
        List<File> files = new List<File>();
        if (IsCompleted())
        {
            foreach (var frame in frames)
            {
                if(!frame.Image.IsFilled && frame.Image.UploadedFile != null)
                    files.Add(frame.Image.UploadedFile);
            }
        }

        return files;
    }

    public List<string> FilledImages()
    {
        List<string> listFilledImages = new List<string>();
        foreach (var frame in frames)
        {
            if (frame.Image.IsActive && frame.Image.UploadedFile == null)
            {
                if (frame.Image.IsFilled)
                {
                    listFilledImages.Add(frame.Image.url);
                }
            }
        }

        return listFilledImages;
    }

    private void OnAddImage(int index, bool isUpdate)
    {
        if (!isUpdate)
        {
            frames[index].Image.Init();
        }
    }

    public void FillImage(string[] urls, Action<UploadFileElement, string, string> action)
    {
        int i = 0;
        foreach (var frame in frames)
        {
            action.Invoke(frame.Image, "pair" + i, urls[i]);
            OnAddImage(i, false);
            i++;
        }
    }

    public void SetId(char id)
    {
        Id = id;
        for (int i =0; i< frames.Length ; i++)
        {
            frames[i].SetCharIndex(Id,i);
        }
    }

    public void ClearPair()
    {
        foreach (var frame in frames)
        {
           frame.OnDeleteButton();
        }
    }

    public void Activate(bool status)
    {
        if (status)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            ClearPair();
        }
    }


}
