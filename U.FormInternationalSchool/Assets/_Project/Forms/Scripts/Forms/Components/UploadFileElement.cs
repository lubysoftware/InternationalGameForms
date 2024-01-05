using System;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = FrostweepGames.Plugins.WebGLFileBrowser.AudioType;

public class UploadFileElement : InputElement
{
    [SerializeField] protected Button uploadFileButton;
    [SerializeField] protected TextMeshProUGUI fileData;
    [SerializeField] protected Transform fileField;
    [SerializeField] protected Button deleteFile;
    [SerializeField] public Image showImage;
    [SerializeField] private bool isMultipleFiles = false;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] protected bool isImage;
    [SerializeField] private Button playAudio;
    [SerializeField] private Button pauseAudio;
    [SerializeField] protected Sprite previewImage;
    [SerializeField] private bool hasErrorMode = true;

    public string url;
    public bool IsFilled;

    private string types;
    private File[] _loadedFiles = null;

    private string fileName;
    private string extension;

    private IFileBrowser _fileBrowser;

    public File UploadedFile => _loadedFiles != null && _loadedFiles.Length > 0 && _loadedFiles[0].data != null
        ? _loadedFiles[0]
        : null;

    private float[] _audioData;
    private string imgData;

    public float[] PreviewAudioData => _audioData;
    public string PreviewImageData => imgData;

    public event Action<UploadFileElement> OnFill;
    public event Action<bool> OnChangeFile;

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
            _fileBrowser.LoadImage(FilesWereOpenedEventHandler, FileOpenFailedEventHandler);
        }
        else
        {
            _fileBrowser.LoadAudio(FilesWereOpenedEventHandler, FileOpenFailedEventHandler);
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
        if(_audioSource != null)
            _audioSource.clip = null;

        IsFilled = false;
        imgData = null;
        _audioData = null;
        AddErrorMessage();
        OnChangeFile?.Invoke(false);
        //   WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content
    }


    protected virtual void FilesWereOpenedEventHandler(File[] files)
    {
        _loadedFiles = files;
        if (_loadedFiles != null && _loadedFiles.Length > 0)
        {
            var file = _loadedFiles[0];

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
                            imgData = GetImageData();
                        }
                        else
                        {
                            fileData.text = $"{file.fileInfo.name}{file.fileInfo.extension}";
                        }

                        // WebGLFileBrowser
                        //     .RegisterFileObject(file
                        //         .ToSprite());
                        // add sprite with texture to cache list. should be used with  fileBrowserFreeMemory() when its no need anymore
                        if (hasErrorMode)
                        {
                            RemoveErrorMessage();
                            DeactivateErrorMode(null, false);
                        }
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
                    if (file.IsAudio(AudioType.OGG) || file.IsAudio(AudioType.OGGVORBIS))
                    {
                        // Debug.Log("File is OGG. " + file.fileInfo.extension);
                        AudioClip clip = file.ToAudioClip();

                        //WebGLFileBrowser.RegisterFileObject(clip);
                        // add audio clip to cache list. should be used with  fileBrowserFreeMemory() when its no need anymore
                        fileData.text = $"{file.fileInfo.fullName}";
                        _audioSource.clip = clip;
                        _audioData = GetAudioData();
                        playAudio.gameObject.SetActive(true);
                        pauseAudio.gameObject.SetActive(true);
                        fileField.gameObject.SetActive(true);
                        if (hasErrorMode)
                        {
                            RemoveErrorMessage();
                            DeactivateErrorMode(null);
                        }
                    }
                    else
                    {
                        Debug.LogError("Não é OGG. " + file.fileInfo.extension);
                    }
                }

                OnChangeFile?.Invoke(true);

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

    public void Clear()
    {
        CleanupButtonOnClickHandler();
    }

    private void FilePopupWasClosedEventHandler()
    {
        if (_loadedFiles == null)
            Debug.LogError("failed to load files");
    }

    private void FileOpenFailedEventHandler(string error)
    {
        Debug.LogError(error);
    }

    private void PlayAudio()
    {
        if (_audioSource.clip != null)
        {
            _audioSource.Play();
        }
    }

    public void PauseAudio()
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
        //this.extension = System.IO.Path.GetExtension(path);
        string[] getName = path.Split("_name.");
        if (getName.Length > 1)
        {
            this.fileName = getName[getName.Length - 1];
        }
        if (isImage)
        {
            SendFilesToAPI.Instance.StartDownloadImage(this, path);
        }
        else
        {
            SendFilesToAPI.Instance.StartDownloadAudio(this, path);
        }
    }

    public virtual void FinishedDownloadFileData(Texture2D texture)
    {
        Sprite s = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        showImage.sprite = s;
        imgData = GetImageData();
        showImage.gameObject.SetActive(true);
        IsFilled = true;
        OnChangeFile?.Invoke(true);
        RemoveErrorMessage();
        if (deleteFile != null)
            deleteFile.gameObject.SetActive(true);
        OnFill?.Invoke(this);
    }

    public void FinishedDownloadFileData(AudioClip clip)
    {
        fileData.gameObject.SetActive(true);
        fileData.text = $"{fileName}";
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        _audioSource.clip = clip;
        _audioData = GetAudioData();
        playAudio.gameObject.SetActive(true);
        pauseAudio.gameObject.SetActive(true);
        IsFilled = true;
        OnChangeFile?.Invoke(true);
        RemoveErrorMessage();
        if (deleteFile != null)
            deleteFile.gameObject.SetActive(true);
        OnFill?.Invoke(this);
    }

    public void DownloadError()
    {
        SucessPanel.Instance.SetText($"Erro ao baixar \"{fileName}\".", SucessPanel.MessageType.ERROR);
        OnFill?.Invoke(this);
    }

    private float[] GetAudioData()
    {
        if (!isImage)
        {
            float[] samples = new float[_audioSource.clip.samples * _audioSource.clip.channels];
            _audioSource.clip.GetData(samples, 0);
            return samples;
        }

        return null;
    }
    
    private string GetImageData()
    {
        if (isImage)
        {
           return FileExtension.TextureToBase64(showImage.sprite.texture);
        }

        return null;
    }
}

public class FileData
{
    public string type;
    public byte[] data;
}