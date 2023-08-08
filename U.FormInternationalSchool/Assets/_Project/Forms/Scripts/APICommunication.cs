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
		StartCoroutine(GetJsonData());
	}

	IEnumerator GetJsonData()
	{
		UnityWebRequest www = UnityWebRequest.Get(Constants.URL_DATABASE + getURL + "?page=1&take=100");

		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		
		else
		{
			Debug.Log(www.downloadHandler.text);
			ImageSeqList seqList = JsonConvert.DeserializeObject<ImageSeqList>(www.downloadHandler.text);
			library.InstantiateGamesList(seqList);
		}
	}

	public void StartDeleteData(int id, GameComponent component)
	{
		StartCoroutine(DeleteData(id,component));
	}
	
	IEnumerator DeleteData(int id, GameComponent component)
	{
		Debug.LogError(Constants.URL_DATABASE + getURL+"/"+id);
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
