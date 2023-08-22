using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indextext;

    [SerializeField] private Button updateImg;

    [SerializeField] private Button delete;

    [SerializeField] private Transform indexField;

    [SerializeField] private bool isActive;

    private int index;

    public int Index => index;

    public bool IsActive => isActive;

    [SerializeField] private bool disableUpdateButton = true;
    
    public ImageElement Image => transform.GetComponentInChildren<ImageElement>();
    void Start()
    {
        isActive = false;
        updateImg.onClick.AddListener(OnUpdateButton);
        delete.onClick.AddListener(OnDeleteButton);
    }

    public void SetIndex(int index)
    {
        this.index = index;
        indextext.text = (index + 1).ToString();
        SetActiveState(false);
    }

    public void SetCharIndex(char letter , int index)
    {
        this.index = index;
        indextext.text = letter + (index + 1).ToString();
    }
    
    public void OnDeleteButton()
    {
        Image.Delete();
    }

    private void OnUpdateButton()
    {
        Image.AddImage();
    }

    public void SetActiveState(bool state)
    {
        isActive = state;
        delete.gameObject.SetActive(state);
        if(disableUpdateButton)
            updateImg.gameObject.SetActive(state);
    }
    
} 
