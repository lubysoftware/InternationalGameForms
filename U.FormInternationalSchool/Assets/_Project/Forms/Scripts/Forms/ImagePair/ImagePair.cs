using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ImagePair : MonoBehaviour
{
    [SerializeField] private ImageFrame[] frames;
    public char Id;
    [SerializeField]
    private TextMeshProUGUI titleText;

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
            if (!frame.IsActive || (frame.Image.UploadedFile == null && !frame.Image.IsFilled))
            {
                return false;
            }
        }

        return true;
    }

    public List<File> GetFiles()
    {
        List<File> files = new List<File>();

        foreach (var frame in frames)
        {
            if(!frame.Image.IsFilled && frame.Image.UploadedFile != null)
                files.Add(frame.Image.UploadedFile);
        }
        

        return files;
    }

    public List<string> FilledImages()
    {
        List<string> listFilledImages = new List<string>();
        foreach (var frame in frames)
        {
            if (frame.Image.isActiveAndEnabled)
            {
                if (frame.Image.UploadedFile == null)
                {
                    if (frame.Image.IsFilled)
                    {
                        listFilledImages.Add(frame.Image.url);
                    }
                    else
                    {
                        listFilledImages.Add(null);
                    }
                   
                }
            }
        }
        
        return listFilledImages;
    }
    
    public List<string> PreviewImages()
    {
        List<string> listFilledImages = new List<string>();
        foreach (var frame in frames)
        {
            Debug.LogError("frame");
            if (frame.Image.isActiveAndEnabled)
            {
                Debug.LogError("isactive");
                listFilledImages.Add(frame.Image.PreviewImageData);
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

    public void SetId(char id, bool showIndex)
    {
        Id = id;
        for (int i =0; i< frames.Length ; i++)
        {
            frames[i].SetCharIndex(Id,i, showIndex);
        }
    }

    public void ShowTitle(string title)
    {
        titleText.text = title;
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
