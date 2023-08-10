using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using File = FrostweepGames.Plugins.WebGLFileBrowser.File;
using UnityEngine.UI;

public class ImageSequencingPanel : MonoBehaviour
{
    [SerializeField] private ImageSequenceElement[] images;
    [SerializeField] private Button newImage;
    public int counter = 0;
    
    void Start()
    {
        for(int i = 0;i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ImageSequenceFrame>().SetIndex(i);
        }
        foreach (ImageSequenceElement el in images)
        {
            el.OnUploadFile += OnAddImage;
            el.OnEndDragElement += OnEndDragElement;
        }
    }

    public void OnNewImageButton()
    {
        transform.GetChild(counter).GetComponentInChildren<ImageSequenceFrame>().Image.AddImage();
    }

    public List<File> GetImages()
    {
        List<File> listImages = new List<File>();
        foreach (Transform child in transform)
        {
            ImageSequenceElement el = child.GetComponent<ImageSequenceFrame>().Image;
            if (el.IsActive && el.UploadedFile != null)
            {
                listImages.Add(el.UploadedFile);
            }
        }
        return listImages;
    }

    public void FillImages(List<SequenceGet> sequences)
    {
        int i = 0;
        int limit = sequences.Count;
        foreach (Transform child in transform)
        {
            ImageSequenceElement el = child.GetComponent<ImageSequenceFrame>().Image;
            if (i < limit)
            {
                el.FillData("seq", sequences[i].imageUrl);
                OnAddImage(false);
            }
            i++;
        }
    }

    private void OnAddImage(bool isUpdate)
    {
        if (!isUpdate)
        {
            ImageSequenceElement elem = transform.GetChild(counter).GetComponentInChildren<ImageSequenceElement>();
            elem.Init(counter);
            elem.OnDelete += OnDelete;
            counter++;
            if (counter == transform.childCount)
            {
                newImage.interactable = false;
            }
        }
    }

    private void OnDelete(ImageSequenceElement element)
    {
        element.OnDelete -= OnDelete;
        OnEndDragElement(element,null,true);
        newImage.interactable = true;
        counter--;
        transform.GetChild(counter).GetComponent<ImageSequenceFrame>().SetActiveState(false);
    }

    private void OnEndDragElement(ImageSequenceElement element, PointerEventData eventData, bool isDelete)
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
                if (transform.GetChild(i).GetComponent<ImageSequenceFrame>().IsActive && transform.GetChild(i).GetComponent<ImageSequenceFrame>().Image != null)
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
                        ImageSequenceElement current = transform.GetChild(i).GetComponent<ImageSequenceFrame>().Image;
                        if (current.IsActive)
                        {
                            current.transform.SetParent(transform.GetChild(i + 1));
                            current.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        }
                    }else{
                        Debug.LogError("nao tem filho");
                    }
                }
            }
            else
            {
                for (int i = newChildPosition; i > element.Index; i--)
                {
                    if (transform.GetChild(i).childCount > 0)
                    {
                        ImageSequenceElement current = transform.GetChild(i).GetComponent<ImageSequenceFrame>().Image;
                        current.transform.SetParent(transform.GetChild(i - 1));
                        current.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    }else{
                        Debug.LogError("nao tem filho");
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

    }
    
}
