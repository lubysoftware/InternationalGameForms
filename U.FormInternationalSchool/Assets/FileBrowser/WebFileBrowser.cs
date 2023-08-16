using System;
using FrostweepGames.Plugins.WebGLFileBrowser;

public class WebFileBrowser : IFileBrowser
{
    public void LoadImage(Action<File[]> onSuccess, Action<string> onError)
    {
        WebGLFileBrowser.FilesWereOpenedEvent += files => onSuccess?.Invoke(files);
        WebGLFileBrowser.FileOpenFailedEvent += onError.Invoke;
        WebGLFileBrowser.FilePopupWasClosedEvent += OnClosePanel;

        WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file to load or use drag & drop");

        WebGLFileBrowser.OpenFilePanelWithFilters(".png,.jpg");
    }

    public void LoadAudio(Action<File[]> onSuccess, Action<string> onError)
    {
        WebGLFileBrowser.FilesWereOpenedEvent += files => onSuccess?.Invoke(files);
        WebGLFileBrowser.FileOpenFailedEvent += onError.Invoke;
        WebGLFileBrowser.FilePopupWasClosedEvent += OnClosePanel;

        WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file to load or use drag & drop");

        WebGLFileBrowser.OpenFilePanelWithFilters(".ogg,.mp3,.wav");
    }

    private void OnClosePanel()
    {
        //WebGLFileBrowser.FilesWereOpenedEvent += ;
    }
}