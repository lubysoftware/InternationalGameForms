using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageSequencingPanel : MonoBehaviour
{
    [SerializeField] private ImageSequenceElement[] images;
    public int counter = 0;

    void Start()
    {
        foreach (ImageSequenceElement el in images)
        {
            el.OnUploadFile += OnAddImage;
            el.OnEndDragElement += OnEndDragElement;
        }
    }

    private void OnAddImage(int index)
    {
        if (index == -1)
        {
            ImageSequenceElement elem = transform.GetChild(counter).GetComponentInChildren<ImageSequenceElement>();
            elem.Init(counter);
            elem.OnDelete += OnDelete;
            counter++;
            if (counter != transform.childCount)
            {
                transform.GetChild(counter).transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private void OnDelete(ImageSequenceElement element)
    {
        element.OnDelete -= OnDelete;
        OnEndDragElement(element,null,true);
        if (counter == transform.childCount)
        {
            transform.GetChild(counter -1).GetChild(0).gameObject.SetActive(true);
        }
        counter--;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            ImageSequenceElement elem = transform.GetChild(i).GetComponentInChildren<ImageSequenceElement>();
            if (elem.IsActive)
            {
                elem.SetIndex(i);
            }
        }
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
                if (transform.GetChild(i).childCount > 0 &&
                    transform.GetChild(i).GetChild(0).GetComponent<ImageSequenceElement>().IsActive)
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
                        Transform current = transform.GetChild(i).GetChild(0);
                        if (current.GetComponent<ImageSequenceElement>().IsActive)
                        {
                            current.transform.SetParent(transform.GetChild(i + 1));
                            current.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                            current.GetComponent<ImageSequenceElement>().IncreaseIndex();
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
                        Transform current = transform.GetChild(i).GetChild(0);
                        current.transform.SetParent(transform.GetChild(i - 1));
                        current.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        current.GetComponent<ImageSequenceElement>().DecreaseIndex();
                        
                    }else{
                        Debug.LogError("nao tem filho");
                    }
                }
            }

            if (transform.GetChild(newChildPosition).childCount == 0)
            {
                element.transform.SetParent(transform.GetChild(newChildPosition));
                element.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                element.GetComponent<ImageSequenceElement>().SetIndex(newChildPosition);
            }
            else
            {
                Debug.LogError("mais de um filho");
            }
        }
        else
        {
            element.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

    }
    
}
