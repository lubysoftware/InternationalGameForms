using System;
using FrostweepGames.Plugins.WebGLFileBrowser;

public interface IFileBrowser
{
    void LoadImage(Action<File[]> onSuccess, Action<string> onError);

    void LoadAudio(Action<File[]> onSuccess, Action<string> onError);
}
