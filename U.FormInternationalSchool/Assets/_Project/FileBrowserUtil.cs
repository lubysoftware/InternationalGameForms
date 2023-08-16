using System;
using System.Collections;
using System.IO;
using LubyLib.Core.Singletons;
using SimpleFileBrowser;
using UnityEngine;

public class FileBrowserUtil : SimpleSingleton<FileBrowserUtil>
{
    public enum FileType
    {
        PNG, GIF, MP4
    }
    
    public void OpenBrowseFile(Action<string> loadSuccess, string title, string buttonText, bool isExport = false, string extension = ".jpg")
    {
        //SimpleFileBrowser.FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        FileBrowser.SetFilters(false, new FileBrowser.Filter("File", extension));
        FileBrowser.SetDefaultFilter(extension);

        StartCoroutine(ShowLoadDialogCoroutine(loadSuccess, isExport, title, buttonText));
    }

    IEnumerator ShowLoadDialogCoroutine(Action<string> loadSuccess, bool isExport = false, string title = "Load Folder",
        string buttonText = "Load")
    {
        if (isExport)
        {
            yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files,
                false, null, null, title, buttonText);
        }
        else
        {
            yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files,
                false, null, null, title, buttonText);
        }

        if (FileBrowser.Success)
        {
            string destinationPath = FileBrowser.Result[0];
            loadSuccess.Invoke(destinationPath);
        }
    }

    public void LoadImage(Action<SelectedFileInfo> loadSuccess, string type)
    {
        FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ) );
        FileBrowser.SetDefaultFilter( "Images" );
        FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );
        FileBrowser.AddQuickLink( "Users", "C:\\Users", null );
        
        StartCoroutine(ShowLoadDialogCoroutine(loadSuccess, type));
    }
    
    IEnumerator ShowLoadDialogCoroutine(Action<SelectedFileInfo> loadSuccess, string type)
    {
        SelectedFileInfo file = new SelectedFileInfo();
        
        yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Image", "Load" );
        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result[0];

            file.fullPath = path;
            file.bytes = FileBrowserHelpers.ReadBytesFromFile(path);
            file.extension = Path.GetExtension(path);
            file.name = FileBrowserHelpers.GetFilename(path).Replace(file.extension, string.Empty);
            file.path = path.Replace(file.name + file.extension, string.Empty);
            
            string destinationPath = Path.Combine(Application.persistentDataPath, file.name + file.extension);
            FileBrowserHelpers.CopyFile(file.fullPath, destinationPath);
            loadSuccess.Invoke(file);
        }
    }
}