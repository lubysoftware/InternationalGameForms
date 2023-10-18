using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indextext;

    [SerializeField] protected Button updateImg;

    [SerializeField] protected Button delete;

    [SerializeField] private Transform indexField;

    [SerializeField] protected bool isActive;

    [SerializeField] protected Button newImageButton;

    protected int index;

    public int Index => index;

    public bool IsActive => isActive;


    public ImageElement Image => transform.GetComponentInChildren<ImageElement>();

    protected virtual void Start()
    {
        updateImg.onClick.AddListener(OnUpdateButton);
        delete.onClick.AddListener(OnDeleteButton);
    }

    public virtual void SetIndex(int index)
    {
        this.index = index;
        indextext.text = (index + 1).ToString();
        SetActiveState(false);
    }

    public void SetCharIndex(char letter, int index, bool showIndex)
    {
        this.index = index;
        if (showIndex)
        {
            SetIndexText(letter);
        }
}

    private void SetIndexText(char letter)
    {
        indextext.text = letter + (index + 1).ToString();
    }

    public virtual void OnDeleteButton()
    {
        SetActiveState(false);
        Image.Delete();
    }

    protected virtual void OnUpdateButton()
    {
        Image.AddImage();
    }

    public virtual void SetActiveState(bool state)
    {
        isActive = state;
        delete.gameObject.SetActive(state);
        updateImg.gameObject.SetActive(state);
        if(newImageButton != null) 
            newImageButton.gameObject.SetActive(!state);
    }
    
} 
