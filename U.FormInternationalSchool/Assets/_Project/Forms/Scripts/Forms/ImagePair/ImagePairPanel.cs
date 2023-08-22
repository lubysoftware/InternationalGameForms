using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class ImagePairPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown pairQtt;

    [SerializeField] private ImagePair[] pairs;

    [SerializeField] private Transform confirmReducePanel;

    [SerializeField] private Button confirmButton;

    public char[] idsList = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
    
    private void Start()
    {
        confirmButton.onClick.AddListener(ShowDropdownQtt);
        pairQtt.onValueChanged.AddListener(OnDropDownChangeValue);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ImagePair>().SetId(idsList[i]);
        }
        ShowDropdownQtt();
    }

    public void OnDropDownChangeValue(int newValue)
    {
        if (CompletedPairs() > pairQtt.value + 2)
        {
            Debug.LogError("tem certeza? voce tem " + CompletedPairs() + " pares preenchidos.");
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
                transform.GetChild(i).SetSiblingIndex(index);
                transform.GetChild(i).GetComponent<ImagePair>().SetId(idsList[i]);
                index++;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i < pairQtt.value+2);
        }
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
        return CompletedPairs() == pairQtt.value+2;
    }

    public List<File> GetAllFiles()
    {
        List<File> files = new List<File>();
        foreach (ImagePair pair in pairs)
        {
            files.AddRange(pair.GetFiles());
        }

        return files;
    }

    public Dictionary<char, List<string>> GetAllFilled()
    {
        Dictionary<char, List<string>> files = new Dictionary<char, List<string>>();
        foreach (ImagePair pair in pairs)
        {
            files.Add(pair.Id, pair.FilledImages());
        }

        return files;
    }
    

    public void FillImages(List<string[]> urls,Action<UploadFileElement, string, string> action)
    {
        int i = 0;
        foreach (ImagePair pair in pairs)
        {
            pair.FillImage(urls[i],action);
            i++;
        }
    }


}
