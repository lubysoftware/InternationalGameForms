using System;
using System.Collections.Generic;
using API;
using FrostweepGames.Plugins.WebGLFileBrowser;
using Proyecto26;
using UnityEngine;
using UnityEngine.Networking;

public class FileUpload : BaseApi
{
    public void UploadFile(List<File> files, Action<List<string>> onSuccess, Action<ErrorProxy> onError)
    {
        var currentRequest = new RequestHelper
        {
            Uri = GetFormattedPath("/file-upload"),
            EnableDebug = DebugMode,
            Retries = 1,
            Timeout = 15,
            FormSections = new List<IMultipartFormSection>()
        };

        foreach (var file in files)
        {
            string contentType = file.fileInfo.extension == "ogg" || file.fileInfo.extension == ".ogg"
                ? "audio/ogg"
                : "image/jpg";
            currentRequest.FormSections.Add(new MultipartFormFileSection("arquivos", file.data, file.fileInfo.fullName, contentType));
        }
        
        Api.Post(currentRequest, onSuccess, onError);
    }
    
    public void DeleteFile(FilesToDelete files, Action<List<string>> onSuccess, Action<ErrorProxy> onError)
    {
        var currentRequest = new RequestHelper
        {
            Uri = GetFormattedPath("/file-upload"),
            EnableDebug = DebugMode,
            Retries = 1,
            Timeout = 15,
            Body = files
        };
    
        Api.Delete(currentRequest, onSuccess, onError);
    }
    
}

[Serializable]
public class FilesToDelete
{
    public List<string> fileNames;
    
    public FilesToDelete(){}

    public FilesToDelete(List<string> fileNames)
    {
        this.fileNames = new List<string>();
        
        foreach (var url in fileNames)
        {
            string fileName = url.Replace("https://edtechprojetos.blob.core.windows.net/arquivos/", string.Empty);
            this.fileNames.Add(fileName);
        }
    }
}