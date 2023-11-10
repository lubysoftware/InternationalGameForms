using System;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class ImagePairPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown pairQtt;

    [SerializeField] private ImagePair[] pairs;

    [SerializeField] private Transform confirmReducePanel;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;
    [SerializeField] private LayoutGroup layoutGroup;
    [SerializeField] private LayoutGroup layoutGroup2;
    public char[] idsList = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

    private int previousDropdown;

    private void Start()
    {
        confirmButton.onClick.AddListener(ShowDropdownQtt);
        denyButton.onClick.AddListener(ResetDropdown);
        pairQtt.onValueChanged.AddListener(OnDropDownChangeValue);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ImagePair>().SetId(idsList[i], true);
        }

        //ShowDropdownQtt();
    }

    public void OnDropDownChangeValue(int newValue)
    {
        int completed = CompletedPairs();
        int value = 0;
        int.TryParse(pairQtt.options[newValue].text, out value);
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
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ImagePair>().IsCompleted())
            {
                transform.GetChild(i).GetComponent<ImagePair>().SetId(idsList[index], true);
                transform.GetChild(i).SetSiblingIndex(index);
                index++;
            }
        }

        int.TryParse(pairQtt.options[pairQtt.value].text, out previousDropdown);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<ImagePair>().Activate(i < previousDropdown);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup2.transform as RectTransform);
    }
    

    private void ResetDropdown()
    {
        pairQtt.value = GetDropdownIndex(previousDropdown);
    }

    public int CompletedPairs()
    {
        int counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<ImagePair>().IsCompleted())
            {
                counter++;
            }
        }

        return counter;
    }

    public bool AllPairsFilled()
    {
        var pair = 0;
        int.TryParse(pairQtt.options[pairQtt.value].text, out pair);
        return CompletedPairs() == pair;
    }

    public List<File> GetAllFiles()
    {
        List<File> files = new List<File>();
        foreach (ImagePair pair in pairs)
        {
            if(pair.GetFiles() != null && pair.GetFiles().Count == 2)
                files.AddRange(pair.GetFiles());
        }
        
        return files;
    }

    public Dictionary<char, List<string>> GetAllFilled()
    {
        Dictionary<char, List<string>> files = new Dictionary<char, List<string>>();
        foreach (ImagePair pair in pairs)
        {
            if(pair.FilledImages() != null && pair.FilledImages().Count > 0)
                files.Add(pair.Id, pair.FilledImages());
        }

        return files;
    }
    
    public Dictionary<char, List<string>> PreviewImages()
    {
        Debug.LogError("preview images");
        Dictionary<char, List<string>> files = new Dictionary<char, List<string>>();
        foreach (ImagePair pair in pairs)
        {
            Debug.LogError("foreach");
            if(pair.PreviewImages() != null && pair.PreviewImages().Count > 0)
                files.Add(pair.Id, pair.PreviewImages());
        }

        return files;
    }
    

    public void FillImages(List<string[]> urls,Action<UploadFileElement, string, string> action)
    {
        pairQtt.SetValueWithoutNotify(GetDropdownIndex(urls.Count));
        ShowDropdownQtt();
        for (int i = 0; i< urls.Count;i++)
        {
            pairs[i].FillImage(urls[i],action);
        }
    }
    
    private int GetDropdownIndex(int qtt)
    {
        return pairQtt.options.FindIndex(x => x.text == qtt.ToString());
    }


}
