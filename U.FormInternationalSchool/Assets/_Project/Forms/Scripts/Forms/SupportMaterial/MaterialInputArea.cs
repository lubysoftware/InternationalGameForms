using System;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialInputArea : MonoBehaviour
{

    [SerializeField] private Button imageButton;

    [SerializeField] private Button textButton;

    [SerializeField] private Button delete;

    [SerializeField] private UploadFileElement imageUploader;

    [SerializeField] private TMP_InputField inputField;

    public bool IsText;

    public string Text => inputField.text;

    public File Image => imageUploader.UploadedFile;

    public UploadFileElement fileUploadEl => imageUploader;

    public event Action<MaterialInputArea> OnDestroy;

    public bool IsEdited = false;
    void Start()
    {
        imageButton.onClick.AddListener(OnClickImage);
        textButton.onClick.AddListener(OnClickText);
        delete.onClick.AddListener(OnDeleteButton);
    }

    private void OnClickText()
    {
        IsEdited = true;
        IsText = true;
        textButton.gameObject.SetActive(false);
        imageButton.gameObject.SetActive(false);
    }
    
    private void OnClickImage()
    {
        IsEdited = true;
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

    public void SetImage(string name, string url)
    {
        OnClickImage();
        imageUploader.FillData(name,url);
    }

    public void SetText(string data)
    {
        OnClickText();
        inputField.gameObject.SetActive(true);
        inputField.text = data;
    }
}
