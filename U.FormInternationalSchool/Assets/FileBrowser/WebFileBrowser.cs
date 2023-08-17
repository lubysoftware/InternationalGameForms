using System;
using FrostweepGames.Plugins.WebGLFileBrowser;

public class WebFileBrowser : IFileBrowser
{
    private Action<File[]> _onSuccess;
    private Action<string> _onError;

    private void SubscribeEvents()
    {
        WebGLFileBrowser.FilesWereOpenedEvent += WebGLFileBrowserOnFilesWereOpenedEvent;
        WebGLFileBrowser.FileOpenFailedEvent += WebGLFileBrowserOnFileOpenFailedEvent;
        WebGLFileBrowser.FilePopupWasClosedEvent += OnClosePanel;
    }

    private void UnsubscribeEvents()
    {
        WebGLFileBrowser.FilesWereOpenedEvent -= WebGLFileBrowserOnFilesWereOpenedEvent;
        WebGLFileBrowser.FileOpenFailedEvent -= WebGLFileBrowserOnFileOpenFailedEvent;
        WebGLFileBrowser.FilePopupWasClosedEvent -= OnClosePanel;
    }

    public void LoadImage(Action<File[]> onSuccess, Action<string> onError)
    {
        _onSuccess = onSuccess;
        _onError = onError;

        SubscribeEvents();

        WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file to load or use drag & drop");

        WebGLFileBrowser.OpenFilePanelWithFilters(".png,.jpg");
    }
    
    public void LoadAudio(Action<File[]> onSuccess, Action<string> onError)
    {
        _onSuccess = onSuccess;
        _onError = onError;
        
        SubscribeEvents();

        WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file to load or use drag & drop");
        WebGLFileBrowser.OpenFilePanelWithFilters(".ogg");
    }

    private void WebGLFileBrowserOnFileOpenFailedEvent(string error)
    {
        _onError?.Invoke(error);
        UnsubscribeEvents();
    }

    private void WebGLFileBrowserOnFilesWereOpenedEvent(File[] files)
    {
        _onSuccess?.Invoke(files);
        UnsubscribeEvents();
    }
    
    private void OnClosePanel()
    {
        //WebGLFileBrowser.FilesWereOpenedEvent += ;
    }
}