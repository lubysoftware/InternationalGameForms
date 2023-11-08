using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using File = FrostweepGames.Plugins.WebGLFileBrowser.File;
using UnityEngine.UI;

public class ImageSequencingPanel : MonoBehaviour
{
    [SerializeField] private ImageElement[] images;
    [SerializeField] private Button newImage;
    public int counter = 0;
    
    void Start()
    {
        for(int i = 0;i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ImageFrame>().SetIndex(i);
        }
        foreach (ImageElement el in images)
        {
            el.OnUploadFile += OnAddImage;
            el.GetComponent<ImageSequenceElement>().OnEndDragElement += OnEndDragElement;
        }
    }

    public void OnNewImageButton()
    {
        transform.GetChild(counter).GetComponentInChildren<ImageFrame>().Image.AddImage();
    }

    public List<File> GetImages()
    {
        List<File> listImages = new List<File>();
        foreach (Transform child in transform)
        {
            ImageElement el = child.GetComponent<ImageFrame>().Image;
            if (el.IsActive && el.UploadedFile != null)
            {
                listImages.Add(el.UploadedFile);
            }
        }
        return listImages;
    }
    
    
    public Dictionary<int,string> FilledImages()
    {
        Dictionary<int, string> listFilledImages = new Dictionary<int, string>();
        foreach (Transform child in transform)
        {
            ImageElement el = child.GetComponent<ImageFrame>().Image;
            if (el.IsActive && el.UploadedFile == null)
            {
                if (el.IsFilled)
                {
                    listFilledImages.Add(el.Index, el.url);
                }
            }
        }

        return listFilledImages;
    }
    
    public Dictionary<int,string> PreviewImages()
    {
        Dictionary<int, string> listFilledImages = new Dictionary<int, string>();
        foreach (Transform child in transform)
        {
            ImageElement el = child.GetComponent<ImageFrame>().Image;
            if (el.IsActive)
            {
                listFilledImages.Add(el.Index, el.GetImageData());
            }
        }

        return listFilledImages;
    }

    public int ImageQtt()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            ImageElement el = child.GetComponent<ImageFrame>().Image;
            if (el.IsActive && (el.UploadedFile != null || el.IsFilled))
            {
                count++;
            }
        }

        return count;
    }

    public void FillImages(List<SequenceGet> sequences, Action<UploadFileElement, string, string> action)
    {
        int i = 0;
        int limit = sequences.Count;
        foreach (Transform child in transform)
        {
            ImageElement el = child.GetComponent<ImageFrame>().Image;
            if (i < limit)
            {
                action.Invoke(el,"seq",sequences[i].imageUrl);
                OnAddImage(false);
            }
            i++;
        }
    }

    private void OnAddImage(bool isUpdate)
    {
        if (!isUpdate)
        {
            ImageElement elem = transform.GetChild(counter).GetComponentInChildren<ImageElement>();
            elem.Init();
            elem.OnDelete += OnDelete;
            counter++;
            if (counter == transform.childCount)
            {
                newImage.interactable = false;
            }
        }
    }

    private void OnDelete(ImageElement element)
    {
        element.OnDelete -= OnDelete;
        OnEndDragElement(element,null,true);
        newImage.interactable = true;
        counter--;
        transform.GetChild(counter).GetComponent<ImageFrame>().SetActiveState(false);
    }

    private void OnEndDragElement(ImageElement element, PointerEventData eventData, bool isDelete)
    {
        int newChildPosition = -1;
        if (isDelete)
        {
            if (transform.childCount - 1 != element.Index)
            {
                newChildPosition = transform.childCount -1;
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<ImageFrame>().IsActive && transform.GetChild(i).GetComponent<ImageFrame>().Image != null)
                {
                    RectTransform currentChild = transform.GetChild(i).GetComponentInChildren<RectTransform>();
                    
                    if (RectTransformUtility.RectangleContainsScreenPoint(currentChild, eventData.position))
                    {
                        newChildPosition = i;
                    }
                }
            }
        }

        if (newChildPosition != -1)
        {
            if (newChildPosition < element.Index)
            {
                for (int i = newChildPosition; i < element.Index; i++)
                {
                    if (transform.GetChild(i).childCount > 0)
                    {
                        ImageElement current = transform.GetChild(i).GetComponent<ImageFrame>().Image;
                        if (current.IsActive)
                        {
                            current.transform.SetParent(transform.GetChild(i + 1));
                            current.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }
                    }else{
                        Debug.Log("nao tem filho");
                    }
                }
            }
            else
            {
                for (int i = newChildPosition; i > element.Index; i--)
                {
                    if (transform.GetChild(i).childCount > 0)
                    {
                        ImageElement current = transform.GetChild(i).GetComponent<ImageFrame>().Image;
                        current.transform.SetParent(transform.GetChild(i - 1));
                        current.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    }else{
                        Debug.Log("nao tem filho");
                    }
                }
            }

            element.transform.SetParent(transform.GetChild(newChildPosition));
            element.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
        else
        {
            element.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        for(int i=0; i < transform.childCount; i++)
        {
            if (i < counter -1)
            {
                transform.GetChild(i).GetComponent<ImageFrame>().SetActiveState(true);
            }
        }
    }

}
