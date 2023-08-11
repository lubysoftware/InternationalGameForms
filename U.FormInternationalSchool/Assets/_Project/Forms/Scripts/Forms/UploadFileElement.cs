using System;
using System.IO;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using File = FrostweepGames.Plugins.WebGLFileBrowser.File;
using FileInfo = FrostweepGames.Plugins.WebGLFileBrowser.FileInfo;

public class UploadFileElement : MonoBehaviour
{
    [SerializeField] protected Button uploadFileButton;
    [SerializeField] protected TextMeshProUGUI fileData;
    [SerializeField] protected Transform fileField;
    [SerializeField] protected Button deleteFile;
    [SerializeField] protected Image showImage;
    [SerializeField] private bool isMultipleFiles = false;
    private AudioSource _audioSource;
    [SerializeField] protected bool isImage;
    [SerializeField] private Button playAudio;
    [SerializeField] private Button pauseAudio;
    [SerializeField] protected Sprite previewImage;

    public string url;
    public bool IsFilled;

    private string types;
    private File[] _loadedFiles = null;

    private string fileName;
    private string extension;
    public File UploadedFile => _loadedFiles != null ? _loadedFiles[0] : null;

    public event Action<UploadFileElement> OnFill;

    private IFileBrowser _fileBrowser;

    protected virtual void Start()
    {
#if UNITY_WEBGL
        _fileBrowser = new WebFileBrowser();
#else
        _fileBrowser = new WindowsFileBrowser();
#endif

        if (uploadFileButton != null)
        {
            uploadFileButton.onClick.AddListener(OpenFileDialogButtonOnClickHandler);
        }

        if (deleteFile != null)
        {
            deleteFile.onClick.AddListener(CleanupButtonOnClickHandler);
        }

        if (isImage)
        {
            types = ".png,.jpg";
        }
        else
        {
            playAudio.onClick.AddListener(PlayAudio);
            pauseAudio.onClick.AddListener(PauseAudio);
            _audioSource = GetComponent<AudioSource>();
            types = ".ogg";
        }

        _audioSource = GetComponent<AudioSource>();

        // if you want to set custom localization for file browser popup -> use that function:
        // fileBrowserSetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file for loading:");
    }

    protected void OpenFileDialogButtonOnClickHandler()
    {
        if (isImage)
        {
            _fileBrowser.LoadImage(FilesWereOpenedEventHandler, Debug.LogError);
        }

        {
            _fileBrowser.LoadAudio(FilesWereOpenedEventHandler, Debug.LogError);
        }
    }

    protected virtual void CleanupButtonOnClickHandler()
    {
        _loadedFiles =
            null; // you have to remove link to file and then GarbageCollector will think that there no links to that object
        if (deleteFile != null)
            deleteFile.gameObject.SetActive(false);

        fileData.text = string.Empty;
        if (isImage)
        {
            showImage.sprite = previewImage;
        }
        else
        {
            showImage.gameObject.SetActive(false);
            deleteFile.gameObject.SetActive(false);
            playAudio.gameObject.SetActive(false);
            pauseAudio.gameObject.SetActive(false);
        }

        IsFilled = false;
        WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content
    }


    protected virtual void FilesWereOpenedEventHandler(File[] files)
    {
        _loadedFiles = files;
        if (_loadedFiles != null && _loadedFiles.Length > 0)
        {
            var file = _loadedFiles[0];

            if (isImage)
            {
                if (_loadedFiles.Length > 1)
                {
                    Debug.LogError($"Loaded files amount: {files.Length}\n");
                }

                foreach (var loadedFile in _loadedFiles)
                {
                    Debug.Log($"Name: {loadedFile.fileInfo.name}{loadedFile.fileInfo.extension}");
                }
            }

            if (deleteFile != null)
                deleteFile.gameObject.SetActive(true);

            if (_loadedFiles.Length == 1)
            {
                if (isImage)
                {
                    if (file.IsImage())
                    {
                        if (showImage != null)
                        {
                            IsFilled = false;
                            showImage.gameObject.SetActive(true);
                            showImage.sprite = file.ToSprite(); // dont forget to delete unused objects to free memory!
                        }
                        else
                        {
                            fileData.text = $"{file.fileInfo.name}{file.fileInfo.extension}";
                        }

                        WebGLFileBrowser
                            .RegisterFileObject(file
                                .ToSprite()); // add sprite with texture to cache list. should be used with  fileBrowserFreeMemory() when its no need anymore
                    }
                }
                else
                {
                    /*  if (file.IsText())
                      {
                          Debug.LogError("bbbbbbbbbbbbbbbbbb");
                          string content = file.ToStringContent();
                          fileData.text += $"\nFile content: {content.Substring(0, Mathf.Min(30, content.Length))}...";
                      }
      */
                    if (file.IsAudio())
                    {
                        AudioClip clip = file.ToAudioClip();

                        //WebGLFileBrowser.RegisterFileObject(clip);
                        // add audio clip to cache list. should be used with  fileBrowserFreeMemory() when its no need anymore
                        fileData.text = $"{file.fileInfo.name}{file.fileInfo.extension}";
                        _audioSource.clip = clip;
                        playAudio.gameObject.SetActive(true);
                        pauseAudio.gameObject.SetActive(true);
                        fileField.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogError("loaded files count = 0");
            }
        }
        else
        {
            Debug.LogError("loaded files = null ou = 0");
        }
    }

    private void PlayAudio()
    {
        if (_audioSource.clip != null)
        {
            _audioSource.Play();
        }
    }

    private void PauseAudio()
    {
        if (_audioSource.clip != null)
        {
            _audioSource.Pause();
        }
    }

    public void FillData(string fileName, string path)
    {
        this.fileName = fileName;
        url = path;
        IsFilled = true;
        //this.extension = System.IO.Path.GetExtension(path);
        if (isImage)
        {
            SendFilesToAPI.Instance.StartDownloadImage(this, path);
        }
        else
        {
            SendFilesToAPI.Instance.StartDownloadAudio(this, path);
        }
    }

    public void FinishedDownloadFileData(Texture2D texture)
    {
        Sprite s = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        showImage.sprite = s;
        showImage.gameObject.SetActive(true);
        if (deleteFile != null)
            deleteFile.gameObject.SetActive(true);
        OnFill?.Invoke(this);
    }

    public void FinishedDownloadFileData(AudioClip clip)
    {
        Debug.Log("entrei");
        fileData.gameObject.SetActive(true);
        fileData.text = $"{fileName}";
        _audioSource.clip = clip;
        playAudio.gameObject.SetActive(true);
        pauseAudio.gameObject.SetActive(true);
        if (deleteFile != null)
            deleteFile.gameObject.SetActive(true);
        OnFill?.Invoke(this);
    }
}

public class FileData
{
    public string type;
    public byte[] data;
}