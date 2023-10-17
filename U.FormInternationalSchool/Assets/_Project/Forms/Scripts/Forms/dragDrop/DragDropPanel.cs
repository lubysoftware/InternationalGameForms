using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragDropPanel : SimpleSingleton<DragDropPanel>
{
    [SerializeField] private Image gridBackground;
    [SerializeField] private Sprite emptyGrid;
    [SerializeField] private UploadFileElement dragBackInput;
    [SerializeField] private ImageFrameDragDrop imageFramePrefab;
    [SerializeField] private TMP_Dropdown imageQtt;
    
    [SerializeField] private Transform confirmReducePanel;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;
    [SerializeField] private Transform positionsContainer;
    
    private int previousDropdown;
    void Start()
    {
        dragBackInput.OnChangeFile += UpdateGridImage;
        confirmButton.onClick.AddListener(ShowDropdownQtt);
        denyButton.onClick.AddListener(ResetDropdown);
        imageQtt.onValueChanged.AddListener(OnDropDownChangeValue);
    }

    private void UpdateGridImage(bool isFilled)
    {
        gridBackground.sprite = isFilled? dragBackInput.UploadedFile.ToSprite():emptyGrid;
    }

   public void OnDropDownChangeValue(int newValue)
    {
        int completed = CompletedImages();
        int value = 0;
        int.TryParse(imageQtt.options[newValue].text, out value);
        if (completed > value)
        {
            int qtt = completed - value;
            if (qtt == 1)
            {
                alertMessage.text = "Deseja excluir o último par preenchido?";
            }
            else
            {
                alertMessage.text =
                    String.Format("Deseja excluir os últimos {0} pares preenchidos?",qtt);
            }
            confirmReducePanel.gameObject.SetActive(true);
            return;
        }

        ShowDropdownQtt();
    }

    private void ShowDropdownQtt()
    {
        int index = 0;
        for (int i = 0; i < gridBackground.transform.childCount; i++)
        {
            if (gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>().IsCompleted())
            {
                gridBackground.transform.GetChild(i).SetSiblingIndex(index);
                index++;
            }
        }

        int newQtt = 0;
        bool updateLayout = true;
        int.TryParse(imageQtt.options[imageQtt.value].text, out newQtt);
        if (newQtt < previousDropdown)
        {
            updateLayout = false;
        }

        previousDropdown = newQtt;
        for (int i = 0; i < gridBackground.transform.childCount; i++)
        {
            gridBackground.transform.GetChild(i).gameObject.GetComponent<ImageFrameDragDrop>().Activate(i < previousDropdown);
            if(updateLayout)
                gridBackground.transform.GetChild(i).localPosition = positionsContainer.GetChild(i).localPosition;
        }
        
    }
    

    private void ResetDropdown()
    {
        imageQtt.value = GetDropdownIndex(previousDropdown);
    }

    public int CompletedImages()
    {
        int counter = 0;
        for (int i = 0; i < gridBackground.transform.childCount; i++)
        {
            if (gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>().IsCompleted())
            {
                counter++;
            }
        }

        return counter;
    }

    public bool AllImagesFilled()
    {
        var images = 0;
        int.TryParse(imageQtt.options[imageQtt.value].text, out images);
        return CompletedImages() == images;
    }
    
    private int GetDropdownIndex(int qtt)
    {
        return imageQtt.options.FindIndex(x => x.text == qtt.ToString());
    }

    public bool CanDropHere(RectTransform rt, int index)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);
        foreach (var pos in v.ToList())
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(gridBackground.GetComponent<RectTransform>(), pos))
            {
                return false;
            }
            
            for (int i = 0; i < gridBackground.transform.childCount; i++)
            {
                if (i == index) break;
                if (gridBackground.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    if (!gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>().CanDropHere(v))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
    



}
