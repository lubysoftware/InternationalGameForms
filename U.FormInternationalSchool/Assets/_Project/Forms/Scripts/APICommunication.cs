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

public class APICommunication : SimpleSingleton<APICommunication>
{
	[SerializeField] private LibraryScreen library;
	private string json;
	private string getURL;

	public event Action<string[]> OnUploadFiles; 


	public void StartDownloadFiles(string url)
	{
		getURL = url;
		StartCoroutine(GetJsonData(url));
	}

	IEnumerator GetJsonData(string url)
	{

		UnityWebRequest www = UnityWebRequest.Get(Constants.URL_DATABASE + url + "?page=1&take=100");

		www.SetRequestHeader("authorization", "Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError(www.error);
		}
		else
		{
			ImageSeqList seqList = JsonConvert.DeserializeObject<ImageSeqList>(www.downloadHandler.text);
			//ImageSeqList seqList = JsonUtility.FromJson<ImageSeqList>(www.downloadHandler.text);
			library.InstantiateGamesList(seqList);
		}

	}
	
	public void StartHealthChecker(string url)
	{
		getURL = url;
		StartCoroutine(GetHealthChecker(url));
	}
	
	IEnumerator GetHealthChecker(string url)
	{
		UnityWebRequest www = UnityWebRequest.Get($"{Constants.URL_DATABASE}health-check");
		www.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		www.SetRequestHeader("Access-Control-Allow-Headers",
			"Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
		www.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
		www.SetRequestHeader("Access-Control-Allow-Origin", "*");
		www.SetRequestHeader("authorization", "Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("request error: " + www.error);
		}

	}

	public void StartDeleteData(int id, GameComponent component)
	{
		StartCoroutine(DeleteData(id,component));
	}
	
	IEnumerator DeleteData(int id, GameComponent component)
	{
		UnityWebRequest www = UnityWebRequest.Delete(Constants.URL_DATABASE + getURL+"/"+id);

		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		
		else
		{
			Debug.Log(www.isDone);
			component.Deactivate();
		}
	}


}
