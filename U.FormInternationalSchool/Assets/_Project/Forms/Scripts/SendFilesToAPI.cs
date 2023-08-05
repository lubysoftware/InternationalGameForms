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

public class SendFilesToAPI : SimpleSingleton<SendFilesToAPI>
{
	private List<File> fileList;
	private string json;
	private string postURL;
	private string getURL;

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

	public void GetGameLibrary(string url)
	{
		getURL = url;
		GetJsonData();
	}

	IEnumerator UploadJson()
	{
		//UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
		UnityWebRequest www = UnityWebRequest.Post(postURL,json, "application/json");
		
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
	
	IEnumerator GetJsonData()
	{
		UnityWebRequest www = UnityWebRequest.Get(getURL);

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
		wr.url = "https://school.gamehub.api.oke.luby.me/file-upload";
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
	
	
}
