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

    public bool IsFilled =>
        FileElement != null ? FileElement.IsFilled : !input.InputField.text.IsNullEmptyOrWhitespace();

    public bool IsCorrect => isCorrect.isOn;

    void Awake()
    {
        FileElement = gameObject.GetComponent<UploadFileElement>();
    }

    public bool IsCompleted()
    {
        if (FileElement != null)
        {
            return gameObject.GetComponent<UploadFileElement>().IsFilled ||
                   gameObject.GetComponent<UploadFileElement>().UploadedFile != null;
        }

        return !input.InputField.text.IsNullEmptyOrWhitespace();
    }

    public bool IsCompleteWithError()
    {
        bool isComplete;
        if (FileElement != null)
        {
            isComplete = gameObject.GetComponent<UploadFileElement>().IsFilled ||
                         gameObject.GetComponent<UploadFileElement>().UploadedFile != null;
            if (!isComplete)
            {
                FileElement.ActivateErrorMode();
            }
            return isComplete;
        }

        isComplete = !input.InputField.text.IsNullEmptyOrWhitespace();
        if (!isComplete)
        {
            input.ActivateErrorMode();
        }
        return isComplete;
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
        if (input.InputField != null)
            input.InputField.text = null;
        if (FileElement != null)
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

    public string GetText()
    {
        return input.InputField.text;
    }

    public void FillAlternative(string url, bool isSelected, FormScreen form, QuestionsGroup.InputType type)
    {
        FileElement = gameObject.GetComponent<UploadFileElement>();
        if (FileElement != null)
        {
            form.loadFileQtt += 1;
            
            form.FillUploadFiles( FileElement,(type +"_"+Index).ToLower(),url);
        }
        else
        {
            input.InputField.text = url;
        }
        if(isSelected)
            ActivateToggle();
    }

}
