using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    public char[] idsList = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };

    private int previousDropdown;

    private void Start()
    {
        confirmButton.onClick.AddListener(ShowDropdownQtt);
        denyButton.onClick.AddListener(ResetDropdown);
        pairQtt.onValueChanged.AddListener(OnDropDownChangeValue);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ImagePair>().SetId(idsList[i]);
        }

        //ShowDropdownQtt();
    }

    public void OnDropDownChangeValue(int newValue)
    {
        int completed = CompletedPairs();
        int value = pairQtt.value + 2;
        if (completed > value)
        {
            alertMessage.text =
                String.Format("Deseja excluir os últimos {1} pares preenchidos?", value, completed - value);
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
                transform.GetChild(i).GetComponent<ImagePair>().SetId(idsList[index]);
                transform.GetChild(i).SetSiblingIndex(index);
                index++;
            }
        }

        previousDropdown = pairQtt.value + 2;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.GetComponent<ImagePair>().Activate(i < previousDropdown);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);
    }
    

    private void ResetDropdown()
    {
        pairQtt.value = previousDropdown;
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
        Debug.LogError("completed pairs: " + CompletedPairs() + " value no pair qtt " + (pairQtt.value+2) );
        return CompletedPairs() == pairQtt.value + 2;
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
    

    public void FillImages(List<string[]> urls,Action<UploadFileElement, string, string> action)
    {
        pairQtt.SetValueWithoutNotify(urls.Count -2);
        ShowDropdownQtt();
        for (int i = 0; i< urls.Count;i++)
        {
            pairs[i].FillImage(urls[i],action);
        }
    }


}
