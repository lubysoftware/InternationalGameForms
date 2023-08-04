using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FrostweepGames.Plugins.WebGLFileBrowser;
using UnityEngine;
using UnityEngine.Networking;
using LubyLib.Core.Singletons;
using Unity.VisualScripting;

public class SendFilesToAPI : SimpleSingleton<SendFilesToAPI>
{
	private List<File> fileList;

	public event Action<string[]> OnUploadFiles; 

	void Start()
	{
		
	}

	public void StartUploadFiles(List<File> files)
	{
		fileList = files;
		StartCoroutine(Teste());
	}

	IEnumerator UploadFiles()
	{
		/*List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
		foreach (File file in fileList)
		{
			formData.Add(new MultipartFormDataSection("arquivos",file.data,"application/multipart/form-data"));
		}
		byte[] boundary = UnityWebRequest.GenerateBoundary();
		fileList.Clear();
		UnityWebRequest www = UnityWebRequest.Post("https://school.gamehub.api.oke.luby.me/file-upload",formData, boundary);*/
	
		WWWForm form = new WWWForm();
		foreach (File file in fileList)
		{
			form.AddBinaryData("arquivos", file.data, file.fileInfo.fullName);
		}

		UnityWebRequest www = UnityWebRequest.Post("https://school.gamehub.api.oke.luby.me/file-upload",form);
		
	//	UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
			OnUploadFiles?.Invoke(www.downloadHandler.text.Split(","));
		}

	}

	IEnumerator Teste()
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
		// Make my request object and add the raw body. Set anything else you need here
		UnityWebRequest wr = new UnityWebRequest();
		UploadHandler uploader = new UploadHandlerRaw(body);
		wr.url = "https://school.gamehub.api.oke.luby.me/file-upload";
		uploader.contentType = contentType;
		wr.uploadHandler = uploader;
		wr.downloadHandler = new DownloadHandlerBuffer();
		//wr.downloadHandler = new DownloadHandlerBuffer();
		wr.method = "POST";
		wr.SetRequestHeader("authorization","Bearer Luby2021");
		yield return wr.Send();
		
		if (wr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(wr.error);
		}
		else
		{
			Debug.Log(wr.downloadHandler.text);
			OnUploadFiles?.Invoke(wr.downloadHandler.text.Split(","));
		}
		
	}
	
	
}
