using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragDropPanel : SimpleSingleton<DragDropPanel>
{
    [SerializeField] private Image gridBackground;
    [SerializeField] private Sprite emptyGrid;
    [SerializeField] private UploadFileElement dragBackInput;
    [SerializeField] private ImageFrameDragDrop imageFramePrefab;
    [SerializeField] private ImageFrameDragDrop textFramePrefab;
    [SerializeField] private TMP_Dropdown imageQtt;
    
    [SerializeField] private Transform confirmReducePanel;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;
    [SerializeField] private Transform positionsContainer;

    [SerializeField] private Toggle isTextToggle;
    [SerializeField] private Toggle isImageToggle;
    [SerializeField] private Toggle isGroupToggle;
    
    [SerializeField] private List<TMP_Dropdown.OptionData> imageOptions;
    [SerializeField] private List<TMP_Dropdown.OptionData> textOptions;
    
    public int previousDropdown;

    private List<FilledElements> filledEls;

    public Canvas canvas;

    private bool isGridSubscribed = false;

    public bool IsGroup => isGroupToggle.isOn;
    public bool IsText => isTextToggle.isOn;
    
    public struct FilledElements
    {
        public int group;
        public Vector3 position;
        public string image;
    }
    void Start()
    {
        if(!isGridSubscribed) 
        {
            dragBackInput.OnChangeFile += UpdateGridImage;
            isGridSubscribed = true;
        }
       
        imageQtt.onValueChanged.AddListener(OnDropDownChangeValue);
       
    }

    private void UpdateGridImage(bool isFilled)
    {
        gridBackground.sprite = isFilled? dragBackInput.showImage.sprite:emptyGrid;
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
            SetConfirmPanelListeners(ShowDropdownQtt,ResetDropdown);
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
            if (gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>().HasImage())
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
            if (gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>().HasImage())
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
    
    public Dictionary<int,string> FilledImages()
    {
        Dictionary<int, string> listFilledImages = new Dictionary<int, string>();
        foreach (Transform child in gridBackground.transform)
        {
            ImageElement el = child.GetComponent<ImageFrameDragDrop>().Image;
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
    
    public List<File> GetImages()
    {
        List<File> listImages = new List<File>();
        foreach (Transform child in gridBackground.transform)
        {
            ImageElement el = child.GetComponent<ImageFrameDragDrop>().Image;
            if (el.IsActive && el.UploadedFile != null)
            {
                listImages.Add(el.UploadedFile);
            }
        }
        return listImages;
    }

    public bool CanDropHere(RectTransform rt, int index)
    {
        Vector3[] cornersArray = new Vector3[4];
        rt.GetWorldCorners(cornersArray);
        foreach (var pos in cornersArray.ToList())
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
                    if (!gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>().CanDropHere(cornersArray))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void UpdateDropdownOptions()
    {
        int completed = CompletedImages();
        if (completed > 0)
        {
            alertMessage.text = "Deseja alterar o tipo de input e descartar os elementos preenchidos?";
            SetConfirmPanelListeners(InstantiateNewElements,CancelChangeInputType);
            confirmReducePanel.gameObject.SetActive(true);
            return;
        }

       InstantiateNewElements();

    }
    
    public void CancelChangeInputType()
    {
        confirmReducePanel.gameObject.SetActive(false);
        if (isTextToggle.isOn)
        {
            isImageToggle.SetIsOnWithoutNotify(true);
        }
        else
        {
            isTextToggle.SetIsOnWithoutNotify(true);
        }
    }

    public void UpdateGroupOptions()
    {
        for (int i = 0; i < gridBackground.transform.childCount; i++)
        {
            gridBackground.transform.GetChild(i).GetComponent<ImageFrameDragDrop>()
                .GroupOptionsStatus(isGroupToggle.isOn);
        }
    }

    private void InstantiateNewElements()
    {
        imageQtt.options.Clear(); 
        imageQtt.options.AddRange(isTextToggle.isOn ? textOptions: imageOptions);
        imageQtt.SetValueWithoutNotify(0);
        if (gridBackground.transform.childCount > 0)
        {
            foreach (Transform child in gridBackground.transform)
            {
                Destroy(child.gameObject);
            }
        }

        ImageFrameDragDrop prefab = isTextToggle.isOn ? textFramePrefab : imageFramePrefab;
        int qtt = 0;
        int.TryParse(imageQtt.options.Last().text, out qtt);

        for (int i = 0; i < textOptions.Count; i++)
        {
            ImageFrameDragDrop imageFrameDragDrop = Instantiate(prefab, gridBackground.transform);
            imageFrameDragDrop.transform.localPosition = positionsContainer.GetChild(i).localPosition;
            imageFrameDragDrop.GroupOptionsStatus(isGroupToggle.isOn);
            imageFrameDragDrop.Activate(i == 0);
            imageFrameDragDrop.SetIndex(i);
        }
        
    }

    public void InstantiateFilledElements(List<DraggableItemJson> filledEls, Action<UploadFileElement> onLoad)
    {
        imageQtt.options.Clear(); 
        imageQtt.options.AddRange(isTextToggle.isOn ? textOptions: imageOptions);
        imageQtt.SetValueWithoutNotify(GetDropdownIndex(filledEls.Count));
        ImageFrameDragDrop prefab = isTextToggle.isOn ? textFramePrefab : imageFramePrefab;
        if (gridBackground.transform.childCount > 0)
        {
            foreach (Transform child in gridBackground.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        for (int i = 0; i < textOptions.Count; i++)
        {
            if (filledEls.Count > i)
            {
                ImageFrameDragDrop imageFrameDragDrop = Instantiate(prefab, gridBackground.transform);
                imageFrameDragDrop.transform.localPosition =
                    new Vector3(filledEls[i].spawnPointX, filledEls[i].spawnPointY);
                imageFrameDragDrop.Image.OnFill += onLoad;
                imageFrameDragDrop.Image.FillData("drag", filledEls[i].imageUrl);
                if (isGroupToggle.isOn)
                {
                    imageFrameDragDrop.SetGroup(filledEls[i].groupId.ToString());
                }
                imageFrameDragDrop.GroupOptionsStatus(isGroupToggle.isOn);
                imageFrameDragDrop.Activate(true);
                imageFrameDragDrop.SetIndex(i);
            }
            else
            {
                ImageFrameDragDrop imageFrameDragDrop = Instantiate(prefab, gridBackground.transform);
                imageFrameDragDrop.transform.localPosition = positionsContainer.GetChild(i).localPosition;
                imageFrameDragDrop.GroupOptionsStatus(isGroupToggle.isOn);
                imageFrameDragDrop.Activate(false);
                imageFrameDragDrop.SetIndex(i);
            }
        }
    }

    private void SetConfirmPanelListeners(UnityAction confirm, UnityAction deny)
    {
        confirmButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirm);
        denyButton.onClick.AddListener(deny);
    }

    public bool BackImageIsFilled()
    {
        if (dragBackInput.UploadedFile == null && !dragBackInput.IsFilled)
        {
            dragBackInput.ActivateErrorMode();
            return false;
        }
        dragBackInput.DeactivateErrorMode(null);  
      
        return true;
    }

    public UploadFileElement BackImage()
    {
        if (!isGridSubscribed)
        {
            dragBackInput.OnChangeFile += UpdateGridImage;
            isGridSubscribed = true;
        }
        return dragBackInput;
    }
    
    public void FillToggles(bool isText, bool isGroup)
    {
        isTextToggle.isOn = isText;
        isGroupToggle.isOn = isGroup;
    }


    public bool CheckIfAllElementsAreFullyComplete()
    {
        foreach (Transform child in gridBackground.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                ImageFrameDragDrop img = child.GetComponent<ImageFrameDragDrop>();
                if (isGroupToggle.isOn)
                {
                    if (!img.HasGroup())
                    {
                        return false;
                    }
                }

                if (!img.HasImage())
                {
                    return false;
                }
            }
        }

        return true;
    }

    public List<DraggableItem> GetAllDraggableItems()
    {
        List<DraggableItem> list = new List<DraggableItem>();
        foreach (Transform child in gridBackground.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                ImageFrameDragDrop img = child.GetComponent<ImageFrameDragDrop>();
                
                list.Add( img.GetItem(isGroupToggle.isOn));
            }
        }

        return list;
    }

}
