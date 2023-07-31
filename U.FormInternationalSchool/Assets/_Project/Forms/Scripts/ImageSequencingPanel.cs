using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSequencingPanel : MonoBehaviour
{
    [SerializeField] private ImageSequenceElement[] images;
    public int counter = 0;

    void Start()
    {
        foreach (ImageSequenceElement el in images)
        {
            el.OnUploadFile += OnAddImage;
        }
    }

    private void OnAddImage(int index)
    {
        if (index == -1)
        {
            ImageSequenceElement elem = transform.GetChild(counter).GetComponent<ImageSequenceElement>();
            elem.Init(counter);
            elem.OnDelete += OnDelete;
            counter++;
            if (counter != transform.childCount)
            {
                transform.GetChild(counter).GetComponent<ImageSequenceElement>().gameObject.SetActive(true);
            }
        }
    }

    private void OnDelete(ImageSequenceElement element)
    {
        element.OnDelete -= OnDelete;
        if (counter == transform.childCount)
        {
            transform.GetChild(counter -1).GetComponent<ImageSequenceElement>().gameObject.SetActive(true);
        }
        counter--;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            ImageSequenceElement elem = transform.GetChild(i).GetComponent<ImageSequenceElement>();
            if (elem.IsActive)
            {
                elem.SetIndex(i);
            }
        }
    }
    
}
