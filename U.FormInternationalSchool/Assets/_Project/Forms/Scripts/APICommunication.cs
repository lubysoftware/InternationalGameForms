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


	public void StartDownloadFiles(string url,int page,int qtt, string filter = "")
	{
		getURL = url;
		StartCoroutine(GetJsonData(url, page, qtt, filter));
	}

	IEnumerator GetJsonData(string url, int page,int qtt, string filter)
	{
		string searchString = "?page="+page+"&take="+qtt+"&search="+filter;
		UnityWebRequest www = UnityWebRequest.Get(Constants.URL_DATABASE + url + searchString);

		www.SetRequestHeader("authorization", "Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError(www.error);
		}
		else
		{
			GameList gameList = JsonConvert.DeserializeObject<GameList>(www.downloadHandler.text);
			//ImageSeqList seqList = JsonUtility.FromJson<ImageSeqList>(www.downloadHandler.text);
			library.InstantiateGamesList(gameList);
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
			SucessPanel.Instance.SetText("Erro ao conectar ao servidor.", SucessPanel.MessageType.ERROR);
		}

	}

	public void StartDeleteData(int id, LibraryScreen screen, string title)
	{
		StartCoroutine(DeleteData(id,screen, title));
	}
	
	IEnumerator DeleteData(int id, LibraryScreen screen, string title)
	{
		UnityWebRequest www = UnityWebRequest.Delete(Constants.URL_DATABASE + getURL+"/"+id);

		www.SetRequestHeader("authorization","Bearer Luby2021");
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			SucessPanel.Instance.SetText("Erro ao deletar \""+title+"\".", SucessPanel.MessageType.ERROR);
		}
		else
		{
			SucessPanel.Instance.SetText("\""+ title+ "\" deletado com sucesso.", SucessPanel.MessageType.SUCCESS);
			Debug.Log(www.isDone);
			screen.OnDeletedGame();
		}
	}
}
