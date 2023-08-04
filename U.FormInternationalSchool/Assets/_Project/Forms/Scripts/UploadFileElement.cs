using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UploadFileElement : MonoBehaviour
{
    [SerializeField] protected Button uploadFileButton;
    [SerializeField] protected TextMeshProUGUI fileData;
    [SerializeField] protected Button deleteFile;
    [SerializeField] protected Image showImage;
    [SerializeField] private bool isMultipleFiles = false;
    private AudioSource _audioSource;
    [SerializeField] protected bool isImage;
    [SerializeField] private Button playAudio;

    private string types;
    private File[] _loadedFiles;

    public File UploadedFile => _loadedFiles[0];

    protected virtual void Start()
    {
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
            _audioSource = GetComponent<AudioSource>();
            types = ".ogg";
        }
        
        // if you want to set custom localization for file browser popup -> use that function:
        // fileBrowserSetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file for loading:");
    }

    private void UnsubscribeEvents()
    {
        WebGLFileBrowser.FilesWereOpenedEvent -= FilesWereOpenedEventHandler;
        WebGLFileBrowser.FilePopupWasClosedEvent -= FilePopupWasClosedEventHandler;
        WebGLFileBrowser.FileOpenFailedEvent -= FileOpenFailedEventHandler;
    }

    private void SubscribeEvents()
    {
        WebGLFileBrowser.FilesWereOpenedEvent += FilesWereOpenedEventHandler;
        WebGLFileBrowser.FilePopupWasClosedEvent += FilePopupWasClosedEventHandler;
        WebGLFileBrowser.FileOpenFailedEvent += FileOpenFailedEventHandler;
    }

    protected void OpenFileDialogButtonOnClickHandler()
    {
        SubscribeEvents();
        WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file to load or use drag & drop");

        // you could paste types like: ".png,.jpg,.pdf,.txt,.json"
        WebGLFileBrowser.OpenFilePanelWithFilters(types, isMultipleFiles);
        // fileBrowserOpenFilePanelWithFilters(fileBrowserGetFilteredFileExtensions(_enteredFileExtensions), isMultipleFiles);
    }

    protected virtual void CleanupButtonOnClickHandler()
    {
        _loadedFiles =
            null; // you have to remove link to file and then GarbageCollector will think that there no links to that object
        if(deleteFile != null)
            deleteFile.gameObject.SetActive(false);

        fileData.text = string.Empty;
        if (isImage)
        {
            showImage.color = new Color(1, 1, 1, 0);
            showImage.sprite = null;
        }
        else
        {
            playAudio.gameObject.SetActive(false);
        }

        WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content
    }


    protected virtual void FilesWereOpenedEventHandler(File[] files)
    {
        UnsubscribeEvents();
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
                    Debug.LogError($"Name: {loadedFile.fileInfo.name}.{loadedFile.fileInfo.extension}");
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
                            showImage.gameObject.SetActive(true);
                            showImage.color = new Color(1, 1, 1, 1);
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
                    if (file.IsAudio(FrostweepGames.Plugins.WebGLFileBrowser.AudioType.OGG))
                    {
                        Debug.LogError("Its audio. try play it");

                        AudioClip clip = file.ToAudioClip();

                        WebGLFileBrowser.RegisterFileObject(clip);
                        // add audio clip to cache list. should be used with  fileBrowserFreeMemory() when its no need anymore
                        fileData.text = $"{file.fileInfo.name}.{file.fileInfo.extension}";
                        _audioSource.clip = clip;
                        playAudio.gameObject.SetActive(true);
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

    private void FilePopupWasClosedEventHandler()
    {
        UnsubscribeEvents();
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


}
