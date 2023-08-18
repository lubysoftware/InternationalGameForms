using System;
using System.Collections.Generic;
using API;
using FrostweepGames.Plugins.WebGLFileBrowser;
using Proyecto26;
using UnityEngine.Networking;

namespace International.Api
{
    public class UtilsApi : BaseApi
    {
        public void CheckHealth(Action<Health> response, Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath("health-check"),
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15
            };
            
            Api.Get(request, response, error);
        }

        public void UploadFiles(List<File> files, Action<List<string>> onSuccess, Action<ErrorProxy> onError)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath("file-upload"),
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
                request.FormSections.Add(new MultipartFormFileSection("arquivos", file.data,
                    file.fileInfo.fullName, contentType));
            }

            Api.Post(request, onSuccess, onError);
        }
        
        public void GetFile(string fileName, Action<File> response, Action<ErrorProxy> error)
        {
            var request = new RequestHelper
            {
                Uri = GetFormattedPath($"file-upload/{fileName}"),
                EnableDebug = DebugMode,
                Retries = 1,
                Timeout = 15
            };
            
            Api.Get(request, response, error);
        }
    }
}