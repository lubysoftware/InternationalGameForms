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
	private List<File> fileList;
	private string json;
	private string postURL;

	public event Action<string[]> OnUploadFiles; 


	public void StartUploadFiles(List<File> files)
	{
		fileList = files;
		StartCoroutine(SendFiles());
	}

	public void StartUploadJson(string json, string url)
	{
		this.json = json;
		postURL = url;
		StartCoroutine(UploadJson());
	}

	IEnumerator UploadJson()
	{
		//UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
		UnityWebRequest www = UnityWebRequest.Post(Constants.URL_DATABASE + postURL,json, "application/json");
		
		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
		}
	}
	
	public void StartUploadJsonUpdate(string json, string url, int id)
	{
		this.json = json;
		postURL = url;
		StartCoroutine(UploadJsonUpdate(id));
	}

	IEnumerator UploadJsonUpdate(int id)
	{
		//UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
		Debug.LogError(Constants.URL_DATABASE + postURL + "/"+id);
		Debug.LogError(json);
		UnityWebRequest www = UnityWebRequest.Put(Constants.URL_DATABASE + postURL + "/"+id, json);

		www.SetRequestHeader("authorization","Bearer Luby2021");
		www.SetRequestHeader("content-type","application/json");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
		}
	}

	IEnumerator SendFiles()
	{
		// read a file and add it to the form
		List<IMultipartFormSection> form = new List<IMultipartFormSection>();
		foreach (var file in fileList)
		{
			form.Add(new MultipartFormFileSection("arquivos", file.data, file.fileInfo.fullName, "image/jpg"));
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
			OnUploadFiles?.Invoke(result.Split("\",\""));
		}
		
	}

	public void StartDownloadImage(UploadFileElement element, string path)
	{
		StartCoroutine(DownloadImage(element, path));
	}

	IEnumerator DownloadImage(UploadFileElement element, string path)
	{
		Debug.LogError(path);
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(path);

		yield return www.SendWebRequest();

		if (www.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
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
		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://stg1atividades.blob.core.windows.net/arquivos/cd3b6cc2-76e1-4266-b4c7-4ac6ea94010a.ogg"
			, AudioType.OGGVORBIS);

		yield return www.SendWebRequest();

		if (www.error != null)
		{
			Debug.Log(www.error);
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
		Debug.LogError(Constants.URL_DATABASE + path +"/"+ id);
		UnityWebRequest www = UnityWebRequest.Get(Constants.URL_DATABASE + path +"/"+ id);
		
		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
			element.FinishDownloadingGame(www.downloadHandler.text);
		}
	}
	
	
}
