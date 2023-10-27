using System.Collections;
using System.Collections.Generic;
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
        letterText.text = letter.ToString();
    }
    
    
}
