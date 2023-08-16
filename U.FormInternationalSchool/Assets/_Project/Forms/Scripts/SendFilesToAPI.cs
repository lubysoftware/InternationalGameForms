using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FrostweepGames.Plugins.WebGLFileBrowser;
using UnityEngine;
using UnityEngine.Networking;
using LubyLib.Core.Singletons;
using Newtonsoft.Json;
using Unity.VisualScripting;
using AudioType = UnityEngine.AudioType;

public class SendFilesToAPI : SimpleSingleton<SendFilesToAPI>
{

	protected override bool DestroyOnLoad => false;
	
	//public event Action<string[]> OnUploadFiles; 
	
	public void StartUploadJson(string json, string url, string title)
	{
		StartCoroutine(UploadJson(url,json, title));
	}

	IEnumerator UploadJson(string postURL, string json, string title)
	{
		//UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
		UnityWebRequest www = UnityWebRequest.Post(Constants.URL_DATABASE + postURL,json, "application/json");
		
		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
			SucessPanel.Instance.SetText("Erro ao salvar o jogo \""+ title +"\".", SucessPanel.MessageType.ERROR);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
			SucessPanel.Instance.SetText("O jogo \""+title +"\" foi salvo com sucesso.", SucessPanel.MessageType.SUCCESS);
		}
	}
	
	public void StartUploadJsonUpdate(string json, string url, int id, string titulo)
	{
		StartCoroutine(UploadJsonUpdate(id, url, json, titulo));
	}

	IEnumerator UploadJsonUpdate(int id, string postURL, string json, string titulo)
	{
		UnityWebRequest www = UnityWebRequest.Put(Constants.URL_DATABASE + postURL + "/"+id, json);

		www.SetRequestHeader("authorization","Bearer Luby2021");
		www.SetRequestHeader("content-type","application/json");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError(www.error);
			SucessPanel.Instance.SetText("Erro ao alterar o jogo \""+ titulo +"\".", SucessPanel.MessageType.ERROR);
		}
		else
		{
			SucessPanel.Instance.SetText("O jogo \""+ titulo +"\" foi alterado com sucesso", SucessPanel.MessageType.SUCCESS);
		}
	}
	
	public void StartUploadFiles(FormScreen screen, List<File> files, bool isBaseForm)
	{
		StartCoroutine(SendFiles(screen, files, isBaseForm));
	}

	IEnumerator SendFiles(FormScreen screen, List<File> fileList, bool isBaseform)
	{
		// read a file and add it to the form
		List<IMultipartFormSection> form = new List<IMultipartFormSection>();
		foreach (var file in fileList)
		{
			Debug.LogError("Send files: file name " + file.fileInfo.fullName+". extension: " + file.fileInfo.extension);
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
		byte[] terminate = Encoding.UTF8.GetBytes(String.Concat("–" + Encoding.UTF8.GetString(boundary)+ "–"));
		
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
		wr.redirectLimit =-1;
		wr.SetRequestHeader("authorization","Bearer Luby2021");
		yield return wr.Send();
		
		if (wr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(wr.error + " uploaded bytes" + wr.uploadedBytes);
		}
		else
		{
			Debug.Log(wr.downloadHandler.text);
			string result = wr.downloadHandler.text;
			result = result.Remove(0,2);
			result = result.Remove(result.Length-2,2);
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
			DownloadHandlerTexture.GetContent(www);
			element.FinishedDownloadFileData(DownloadHandlerTexture.GetContent(www));
		}
	}
	
	public void StartDownloadAudio(UploadFileElement element, string path)
	{
		StartCoroutine(DownloadAudio(element, path));
	}

	IEnumerator DownloadAudio(UploadFileElement element, string path)
	{
		Debug.LogError(path);
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
		UnityWebRequest www = UnityWebRequest.Get(Constants.URL_DATABASE + path +"/"+ id);
		
		www.SetRequestHeader("authorization","Bearer Luby2021");
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
	
	
}
