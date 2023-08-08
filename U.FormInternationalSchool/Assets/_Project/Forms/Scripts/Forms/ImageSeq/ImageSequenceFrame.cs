using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageSequenceFrame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indextext;

    [SerializeField] private Button updateImg;

    [SerializeField] private Button delete;

    [SerializeField] private bool isActive;

    private int index;

    public int Index => index;

    public bool IsActive => isActive;
    
    public ImageSequenceElement Image => transform.GetComponentInChildren<ImageSequenceElement>();
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
    private void OnDeleteButton()
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
        updateImg.gameObject.SetActive(state);
    }
    
}
