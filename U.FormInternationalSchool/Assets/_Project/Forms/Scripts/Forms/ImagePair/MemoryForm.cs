using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MemoryForm : FormScreen
{
    [SerializeField] private MemoryPanel panel;
    private int pairsQtt;
    private Dictionary<char,List<string>> filledImages;
    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseJsonGet>(text));
            FillGameData(JsonConvert.DeserializeObject<MemoryJsonGet>(text));
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
        if (!panel.AllPairsFilled())
        {
            ShowError("Todos os pares devem ser preenchidos.", ErrorType.CUSTOM, null);
            return;
        }
        
        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.LogError("serialize game " + urls);

        List<Pair> listPair = new List<Pair>();
        if (filledImages.Count == pairsQtt || urls == null)
        {
            for(int i=0; i< pairsQtt; i++)
            {
                listPair.Add(new Pair() { firstImageUrl = filledImages[panel.idsList[i]][0], secondImageUrl = filledImages[panel.idsList[i]][1] });
            }
        }
        else
        {
            int urlIndex = 0;
            for(int i = 0; i< pairsQtt; i++)
            {
                if (filledImages.ContainsKey(panel.idsList[i]))
                {
                    if (filledImages[panel.idsList[i]].Count == 2)
                    {
                        listPair.Add(new Pair()
                            { firstImageUrl = filledImages[panel.idsList[i]][0], secondImageUrl = filledImages[panel.idsList[i]][1] });
                    }
                    else if (filledImages[panel.idsList[i]].Count == 1)
                    {
                        listPair.Add(
                            new Pair() { firstImageUrl = filledImages[panel.idsList[i]][0], secondImageUrl = urls[urlIndex] });
                        urlIndex++;
                    }
                }
                else
                {
                    listPair.Add(new Pair() { firstImageUrl = urls[urlIndex], secondImageUrl = urls[urlIndex + 1] });
                    urlIndex +=2;
                }
                Debug.LogError("url index " + urlIndex + " completed pairs? " +  panel.CompletedPairs());
            }
        }
       

        FormMatchCard completeForm = new FormMatchCard()
        {
            game = this.game,
            gameData =  new MatchCard()
            {
                failPenalty = 0,
                cardPairs = listPair
            }
        };

       
        string json = JsonConvert.SerializeObject(completeForm);
        Debug.LogError(json);
        if (isEdit)
        {
            SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.text, this);
        }
        else
        {
            SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.text, this);
        }
    }

    private void FillGameData(MemoryJsonGet json)
    {
        List<string[]> urls = new List<string[]>();
        for (int i = 0; i < json.cardPairs.Count; i++)
        {
            string[] urlPair = new[] { json.cardPairs[i].firstImageUrl, json.cardPairs[i].secondImageUrl };
            urls.Add(urlPair);
        }
        panel.FillImages(urls, FillUploadFiles);
        loadFileQtt = loadFileQtt + urls.Count * 2;
        CheckIfMaxQtt();
    }

}

[Serializable]
public class MatchCard
{
    public int failPenalty;
    public List<Pair> cardPairs;
}

[Serializable]
public class FormMatchCard
{
    public FormBase game;
    public MatchCard gameData;
}


