using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using LubyLib.Core.Singletons;
using Newtonsoft.Json;

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
			SuccessPanel.Instance.SetText("Erro ao conectar ao servidor.", SuccessPanel.MessageType.ERROR);
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
			SuccessPanel.Instance.SetText("Erro ao deletar \""+title+"\".", SuccessPanel.MessageType.ERROR);
		}
		else
		{
			SuccessPanel.Instance.SetText("\""+ title+ "\" deletado com sucesso.", SuccessPanel.MessageType.SUCCESS);
			Debug.Log(www.isDone);
			screen.OnDeletedGame();
		}
	}
}
