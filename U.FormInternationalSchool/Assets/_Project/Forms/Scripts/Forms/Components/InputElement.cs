using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private Image errorSymbol;
    [SerializeField] private Sprite fieldErrorMode;
    [SerializeField] private Sprite fieldNormalMode;
    [SerializeField] private Image field;

    public bool Null = false;

    public TMP_InputField InputField => GetComponentInChildren<TMP_InputField>();
    private string title = "";

    private void OnEnable()
    {
        if (errorMessage != null)
        {
            title = errorMessage.text;
            AddErrorMessage();
        }
    }

    public virtual void ActivateNullMode(bool cleanInput = false)
    {
        AddErrorMessage();
        errorSymbol.gameObject.SetActive(true);
        field.sprite = fieldErrorMode;
        if (cleanInput && InputField != null)
        {
            InputField.text = null;
        }

        Null = true;
    }

    public void DeactivateErrorMode()
    {
        DeactivateErrorMode(fieldNormalMode);
        Null = false;
    }

    public virtual void DeactivateErrorMode(Sprite normal,bool changeSprite = true)
    {
        RemoveErrorMessage();
        if(errorSymbol != null)
          errorSymbol.gameObject.SetActive(false);
        if (changeSprite)
            field.sprite = normal != null? normal : fieldNormalMode;
    }

    public void AddErrorMessage()
    {
        if(errorMessage != null && !errorMessage.text.Contains("*"))
            errorMessage.text = title + "<color=\"red\">*</color>";
    }

    public void RemoveErrorMessage()
    {
        //if(errorMessage != null)
            //errorMessage.text = title;
    }
}
