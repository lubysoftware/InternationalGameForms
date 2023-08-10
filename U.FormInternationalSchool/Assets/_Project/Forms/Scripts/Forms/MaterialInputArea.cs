using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialInputArea : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Button imageButton;

    [SerializeField] private Button textButton;

    [SerializeField] private Button delete;

    [SerializeField] private UploadFileElement imageUploader;

    public bool IsText;

    public string Text => text.text;

    public File Image => imageUploader.UploadedFile;

    public event Action<MaterialInputArea> OnDestroy;
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
        imageButton.gameObject.SetActive(false);
        imageUploader.gameObject.SetActive(true);
    }

    private void OnDeleteButton()
    {
        OnDestroy?.Invoke(this);
        Destroy(this.gameObject);
    }
}