using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
	
	void Start()
	{
		StartCoroutine(Upload());
	}

	IEnumerator Upload()
	{
		/*WWWForm form = new WWWForm();
		form.AddField("myField", "myData");*/

		UnityWebRequest www = UnityWebRequest.Get("https://school.gamehub.api.oke.luby.me/health-check");
		//imagem -> POST https://school.gamehub.api.oke.luby.me/health-check/file-upload
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
}
