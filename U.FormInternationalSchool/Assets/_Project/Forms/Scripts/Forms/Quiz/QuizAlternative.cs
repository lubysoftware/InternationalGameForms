using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizAlternative : MonoBehaviour
{
    public char Index;

    [SerializeField] private Toggle isCorrect;
    [SerializeField] private TextMeshProUGUI letterText;
    [SerializeField] private InputElement input;
    private UploadFileElement FileElement;
    
    public bool IsFilled => FileElement != null? FileElement.IsFilled: !input.InputField.text.IsNullEmptyOrWhitespace();
    void Awake()
    {
        FileElement = gameObject.GetComponent<UploadFileElement>();
    }

    public bool IsCompleted()
    {
        if (FileElement != null)
        {
            return gameObject.GetComponent<UploadFileElement>().IsFilled || gameObject.GetComponent<UploadFileElement>().UploadedFile != null;
        }
        return !input.InputField.text.IsNullEmptyOrWhitespace();
    }

    public string GetFilledUrl()
    {
        if (FileElement != null)
        {
            if (gameObject.GetComponent<UploadFileElement>().IsFilled)
            {
                return gameObject.GetComponent<UploadFileElement>().url;
            }
        }
        return "";
    }
    
    public File GetFile()
    {
        if (FileElement != null)
        {
            if (gameObject.GetComponent<UploadFileElement>().UploadedFile != null)
            {
                return gameObject.GetComponent<UploadFileElement>().UploadedFile;
            }
        }
        return null;
    }
    
    public bool Deactivate()
    {
        gameObject.SetActive(false);
        bool correct = isCorrect.isOn;
        isCorrect.isOn = false;
        if(input.InputField != null)
            input.InputField.text = null;
        if(FileElement != null)
            FileElement.Clear();
        return correct;
    }

    public void ActivateToggle()
    {
        isCorrect.isOn = true;
    }

    public void SetIndex(char letter)
    {
        Index = letter;
        letterText.text = letter.ToString();
    }
    
    
}
