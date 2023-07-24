using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialInputARea : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Button imageButton;

    [SerializeField] private Button textButton;

    [SerializeField] private Button delete;

    public bool IsText;

    public string Text => text.text;

    public event Action<MaterialInputARea> OnDestroy;
    void Start()
    {
        text.gameObject.SetActive(false);
        imageButton.onClick.AddListener(OnClickImage);
        textButton.onClick.AddListener(OnClickText);
        delete.onClick.AddListener(OnDeleteButton);
    }

    private void OnClickText()
    {
        IsText = true;
        textButton.gameObject.SetActive(false);
        imageButton.gameObject.SetActive(false);
        text.gameObject.SetActive(true);
    }
    
    private void OnClickImage()
    {
        IsText = false;
        textButton.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    private void OnDeleteButton()
    {
        OnDestroy?.Invoke(this);
        Destroy(this.gameObject);
    }
}
