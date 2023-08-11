using System;
using System.IO;
using System.Threading.Tasks;
using SimpleFileBrowser;
using UnityEngine;

public class WindowsFileBrowser : IFileBrowser
{
    public void LoadImage(Action<FrostweepGames.Plugins.WebGLFileBrowser.File[]> onSuccess,
        Action<string> onError)
    {
        string extension = "jpg";

        FileBrowser.SetFilters(false, new FileBrowser.Filter("File", extension));
        FileBrowser.SetDefaultFilter("Images");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        LoadFile("Carregar Imagem", "Selecionar", onSuccess, onError);
    }

    public void LoadAudio(Action<FrostweepGames.Plugins.WebGLFileBrowser.File[]> onSuccess, Action<string> onError)
    {
        string extension = "ogg";

        FileBrowser.SetFilters(false, new FileBrowser.Filter("File", extension));
        FileBrowser.SetDefaultFilter("Audios");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        LoadFile("Carregar Áudio", "Selecionar", onSuccess, onError);
    }

    private async Task LoadFile(string title, string button,
        Action<FrostweepGames.Plugins.WebGLFileBrowser.File[]> onSuccess,
        Action<string> onError)
    {
        await FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, string.Empty, title, button);

        if (!FileBrowser.Success)
        {
            onError?.Invoke("Erro ao carregar a image.");
            return;
        }

        SelectedFileInfo file = new SelectedFileInfo();

        string path = FileBrowser.Result[0];

        file.fullPath = path;
        file.bytes = FileBrowserHelpers.ReadBytesFromFile(path);
        file.extension = Path.GetExtension(path);
        file.name = FileBrowserHelpers.GetFilename(path).Replace(file.extension, string.Empty);
        file.path = path.Replace(file.name + file.extension, string.Empty);

        string destinationPath = Path.Combine(Application.persistentDataPath, file.name + file.extension);
        FileBrowserHelpers.CopyFile(file.fullPath, destinationPath);

        FrostweepGames.Plugins.WebGLFileBrowser.File[] files = new FrostweepGames.Plugins.WebGLFileBrowser.File[1];
        files[0] = new FrostweepGames.Plugins.WebGLFileBrowser.File
        {
            data = file.bytes,
            fileInfo = new FrostweepGames.Plugins.WebGLFileBrowser.FileInfo
            {
                path = file.path,
                fullName = Path.Combine(file.path, file.name + file.extension),
                name = file.name,
                extension = file.extension
            }
        };

        onSuccess.Invoke(files);
    }
}