using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using API;
using FrostweepGames.Plugins.WebGLFileBrowser;
using UnityEngine;
using UnityEngine.Networking;
using LubyLib.Core.Singletons;
using Newtonsoft.Json;
using Proyecto26;
using AudioType = UnityEngine.AudioType;

public class SendFilesToAPI : SimpleSingleton<SendFilesToAPI>
{
    protected override bool DestroyOnLoad => false;

    //public event Action<string[]> OnUploadFiles; 

    public void StartUploadJson(string json, string url, string title, FormScreen screen, Action<string> onSuccess)
    {
        StartCoroutine(UploadJson(url, json, title, screen, onSuccess));
    }

    IEnumerator UploadJson(string postURL, string json, string title, FormScreen screen, Action<string> onSuccess)
    {
        //UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
        UnityWebRequest www = UnityWebRequest.Post(Constants.URL_DATABASE + postURL, json, "application/json");

        www.SetRequestHeader("authorization", GlobalSettings.Instance.UserToken);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            SucessPanel.Instance.SetText("Erro ao salvar o jogo \"" + title + "\".", SucessPanel.MessageType.ERROR);
            screen.FinishSaveData();
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            SucessPanel.Instance.SetText("O jogo \"" + title + "\" foi salvo com sucesso.",
                SucessPanel.MessageType.SUCCESS);
            onSuccess?.Invoke(www.downloadHandler.text);
            screen.BackButton();
        }
    }
    

    public void StartUploadJsonUpdate(string json, string url, int id, string titulo, FormScreen screen, Action<string> onSuccess)
    {
        StartCoroutine(UploadJsonUpdate(id, url, json, titulo, screen, onSuccess));
    }

    IEnumerator UploadJsonUpdate(int id, string postURL, string json, string titulo, FormScreen screen, Action<string> onSuccess)
    {
        UnityWebRequest www = UnityWebRequest.Put(Constants.URL_DATABASE + postURL + "/" + id, json);

        www.SetRequestHeader("authorization", GlobalSettings.Instance.UserToken);
        www.SetRequestHeader("content-type", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            SucessPanel.Instance.SetText("Erro ao alterar o jogo \"" + titulo + "\".", SucessPanel.MessageType.ERROR);
            screen.FinishSaveData();
        }
        else
        {
            SucessPanel.Instance.SetText("O jogo \"" + titulo + "\" foi alterado com sucesso",
                SucessPanel.MessageType.SUCCESS);
            
            onSuccess?.Invoke(www.downloadHandler.text);
            screen.FinishSaveData();
        }
    }

    public void StartUploadFiles(FormScreen screen, List<File> files, bool isBaseForm)
    {
        SendFilesPack(screen, files, isBaseForm);
        //StartCoroutine(SendFiles(screen, files, isBaseForm));
    }

    private void SendFilesPack(FormScreen screen, List<File> fileList, bool isBaseForm)
    {
        RestClient.DefaultRequestHeaders["Authorization"] = GlobalSettings.Instance.UserToken;

        APIFactory.GetApi<FileUpload>().UploadFile(fileList, list =>
        {
            string[] result = list.ToArray();
            
            if (isBaseForm)
            {
                screen.SerializeBaseFormData(result);
            }
            else
            {
                screen.SerializeGameData(result);
            }
        }, error =>
        {
            Debug.LogError(error.message);
            SucessPanel.Instance.SetText("Houve um erro ao enviar os arquivos: "+ error.message,
                SucessPanel.MessageType.ERROR);
            screen.FinishSaveData();
        });
    }

    IEnumerator SendFiles(FormScreen screen, List<File> fileList, bool isBaseform)
    {
        // read a file and add it to the form
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();
        foreach (var file in fileList)
        {
            Debug.Log("Send files: file name " + file.fileInfo.fullName + " . extension: " +
                           file.fileInfo.extension);
            if (file.fileInfo.extension == "ogg" || file.fileInfo.extension == ".ogg")
            {
                form.Add(new MultipartFormFileSection("arquivos", file.data, file.fileInfo.fullName, "audio/ogg"));
            }
            else
            {
                form.Add(new MultipartFormFileSection("arquivos", file.data, file.fileInfo.fullName, "image/jpg"));
            }
        }

        // generate a boundary then convert the form to byte[]
        byte[] boundary = UnityWebRequest.GenerateBoundary();
        byte[] formSections = UnityWebRequest.SerializeFormSections(form, boundary);

        // my termination string consisting of CRLF--{boundary}--
        byte[] terminate = Encoding.UTF8.GetBytes(String.Concat("–" + Encoding.UTF8.GetString(boundary) + "–"));

        // Make my complete body from the two byte arrays
        byte[] body = new byte[formSections.Length + terminate.Length];
        Buffer.BlockCopy(formSections, 0, body, 0, formSections.Length);
        Buffer.BlockCopy(terminate, 0, body, formSections.Length, terminate.Length);

        // Set the content type - NO QUOTES around the boundary
        string contentType = String.Concat("multipart/form-data; boundary=", Encoding.UTF8.GetString(boundary));

        // Make my request object and add the raw body
        UnityWebRequest wr = new UnityWebRequest();
        UploadHandler uploader = new UploadHandlerRaw(body);
        wr.url = Constants.URL_UPLOAD_MEDIA;
        uploader.contentType = contentType;
        wr.uploadHandler = uploader;
        wr.downloadHandler = new DownloadHandlerBuffer();
        wr.method = "POST";
        wr.redirectLimit = -1;
        wr.SetRequestHeader("authorization", GlobalSettings.Instance.UserToken);
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            if (isBaseform)
            {
                SucessPanel.Instance.SetText("Erro ao fazer upload de arquivos. \n" + wr.error,
                    SucessPanel.MessageType.ERROR);
            }
            else
            {
                SucessPanel.Instance.SetText("Erro ao fazer upload de arquivos da seção de sequenciamento de imagens. \n" + wr.error, SucessPanel.MessageType.ERROR);
            }
        }
        else
        {
            Debug.Log(wr.downloadHandler.text);
            string result = wr.downloadHandler.text;
            result = result.Remove(0, 2);
            result = result.Remove(result.Length - 2, 2);
            Debug.Log(result);
            if (isBaseform)
            {
                screen.SerializeBaseFormData(result.Split("\",\""));
            }
            else
            {
                screen.SerializeGameData(result.Split("\",\""));
            }
        }
    }

    public void StartDownloadImage(UploadFileElement element, string path)
    {
        StartCoroutine(DownloadImage(element, path));
    }

    IEnumerator DownloadImage(UploadFileElement element, string path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

        yield return www.SendWebRequest();

        if (www.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(www.error);
            element.DownloadError();
        }
        else
        {
            element.FinishedDownloadFileData(DownloadHandlerTexture.GetContent(www));
        }
    }
    
    public void StartDownloadImageForm(FormScreen form, string path)
    {
        StartCoroutine(DownloadImageForm(form, path));
    }

    IEnumerator DownloadImageForm(FormScreen form, string path)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

        yield return www.SendWebRequest();

        if (www.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            form.FinishDownloadImage(DownloadHandlerTexture.GetContent(www));
        }
    }

    public void StartDownloadAudio(UploadFileElement element, string path)
    {
        StartCoroutine(DownloadAudio(element, path));
    }

    IEnumerator DownloadAudio(UploadFileElement element, string path)
    {
#if (UNITY_WEBGL || FG_FB_WEBGL) && !UNITY_EDITOR
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path
			, AudioType.AUDIOQUEUE);
#endif
#if UNITY_EDITOR
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path
            , AudioType.OGGVORBIS);
#endif
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            Debug.LogError(www.error);
            element.DownloadError();
        }
        else
        {
            element.FinishedDownloadFileData(DownloadHandlerAudioClip.GetContent(www));
        }
    }


    public void StartDownloadGame(FormScreen element, string path, int id)
    {
        StartCoroutine(DownloadGame(element, path, id));
    }

    IEnumerator DownloadGame(FormScreen element, string path, int id)
    {
        UnityWebRequest www = UnityWebRequest.Get(Constants.URL_DATABASE + path + "/" + id);

        www.SetRequestHeader("authorization", GlobalSettings.Instance.UserToken);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            element.FinishDownloadingGame(www.downloadHandler.text);
        }
    }
    
    public void DeleteOldFiles(List<string> filesToDelete)
    {
        if (filesToDelete == null || filesToDelete.Count == 0)
            return;
        
        RestClient.DefaultRequestHeaders["Authorization"] = GlobalSettings.Instance.UserToken;
        
        APIFactory.GetApi<FileUpload>().DeleteFile(new FilesToDelete(filesToDelete),
            list =>
            {
                Debug.Log($"Files deleted: {list.Count}");
            },
            errorProxy =>
            {
                Debug.LogError("error " + errorProxy.message);
            });
    }
}