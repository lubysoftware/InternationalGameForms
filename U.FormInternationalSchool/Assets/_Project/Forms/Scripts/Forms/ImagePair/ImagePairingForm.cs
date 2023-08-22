using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class ImagePairingForm : FormScreen
{
    [SerializeField] private ImagePairPanel panel;
    [SerializeField] private TMP_InputField failsPenalty;
    private int pairsQtt;
    private Dictionary<char,List<string>> filledImages;
    
    private int failsPenaltyValue = 0;
    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseJsonGet>(text));
            FillGameData(JsonConvert.DeserializeObject<ImagePairJsonGet>(text));
        }
    }


    protected override void SendGameFiles()
    {   filledImages = panel.GetAllFilled();
        pairsQtt = panel.CompletedPairs();
        if (pairsQtt < 2)
        {
            ShowError("Deve conter no minimo 2 pares.", ErrorType.CUSTOM, null);
        }
        else
        {
            if (panel.GetAllFiles() != null && panel.GetAllFiles().Count > 0)
            {
                SendFilesToAPI.Instance.StartUploadFiles(this, panel.GetAllFiles(), false);
            }else
            {
                SerializeGameData(null);
            }
        }
    }
    
    protected override void CheckGameFields()
    {
        if (failsPenalty.text.IsNullEmptyOrWhitespace())
        {
            ShowError("Pontuação descontada por erro", ErrorType.EMPTY, null);
            return;
        }

        if (!panel.AllPairsFilled())
        {
            ShowError("Todos os pares devem ser preenchidos.", ErrorType.CUSTOM, null);
            return;
        }
        
        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.LogError("serialize game" + urls);
        
        List<Pair> listPair = new List<Pair>();
        if (filledImages.Count == pairsQtt)
        {
            foreach (char id in panel.idsList)
            {
                listPair.Add(new Pair() { firstImageUrl = filledImages[id][0], secondImageUrl = filledImages[id][1] });
            }
        }
        else
        {
            int urlIndex = 0;
            foreach (char id in panel.idsList)
            {
                if (filledImages.ContainsKey(id))
                {
                    if (filledImages[id].Count == 2)
                    {
                        listPair.Add(new Pair() { firstImageUrl = filledImages[id][0], secondImageUrl = filledImages[id][1] });
                    }
                    else if(filledImages[id].Count == 1)
                    {
                        listPair.Add(new Pair() { firstImageUrl = filledImages[id][0], secondImageUrl = urls[urlIndex] });
                        urlIndex++;
                    }
                }
                else
                {
                    listPair.Add(new Pair() { firstImageUrl = urls[urlIndex], secondImageUrl = urls[urlIndex+1] });
                    urlIndex = +2;
                }
            }
        }
       

        FormImagePairing completeForm = new FormImagePairing()
        {
            game = this.game,
            gameData =  new ImagePairing()
            {
                failPenalty = failsPenaltyValue,
                pairs = listPair
            }
        };

       
        string json = JsonConvert.SerializeObject(completeForm);
        if (isEdit)
        {
            SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.text, this);
        }
        else
        {
            SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.text, this);
        }
    }

    private void FillGameData(ImagePairJsonGet json)
    {
        failsPenalty.text = json.ImageParing.failPenalty.ToString();
        List<string[]> urls = new List<string[]>();
        for (int i = 0; i < json.ImageParing.Pair.Count; i++)
        {
            string[] urlPair = new[] { json.ImageParing.Pair[i].firstImageUrl, json.ImageParing.Pair[i].secondImageUrl };
            urls.Add(urlPair);
        }
        panel.FillImages(urls, FillUploadFiles);
    }

}
[Serializable]
public struct Pair
{
    public string firstImageUrl;
    public string secondImageUrl;
}

[Serializable]
public class ImagePairing
{
    public int failPenalty;
    public List<Pair> pairs;
}

[Serializable]
public class FormImagePairing
{
    public FormBase game;
    public ImagePairing gameData;
}

