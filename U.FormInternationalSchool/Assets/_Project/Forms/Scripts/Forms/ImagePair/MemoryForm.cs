using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
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
    private Dictionary<char, List<string>> filledImages;
    [SerializeField] private ImageFrame backCardImage;
    [SerializeField] private TMP_InputField oneStar;
    [SerializeField] private TMP_InputField twoStars;
    [SerializeField] private TMP_InputField threeStars;

    private string backImagePath = "";

    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseJsonGet>(text));
            FillGameData(JsonConvert.DeserializeObject<MemoryJsonGet>(text));
        }
    }


    protected override void SendGameFiles()
    {
        List<File> files = new List<File>();
        if (backCardImage.Image.UploadedFile != null)
        {
            files.Add(backCardImage.Image.UploadedFile);
            backImagePath = "";
        }
        else
        {
            if (backCardImage.Image.IsFilled)
            {
                backImagePath = backCardImage.Image.url;
            }
            else
            {
                ShowError("Imagem de verso da carta.", ErrorType.EMPTY, null);
                return;
            }
        }

        filledImages = panel.GetAllFilled();
        pairsQtt = panel.CompletedPairs();
        if (pairsQtt < 2)
        {
            ShowError("Deve conter no minimo 2 pares.", ErrorType.CUSTOM, null);
        }
        else
        {
            if (panel.GetAllFiles() != null && panel.GetAllFiles().Count > 0)
            {
                files.AddRange(panel.GetAllFiles());
            }

            if (files.Count > 0)
            {
                SendFilesToAPI.Instance.StartUploadFiles(this, files, false);
            }
            else
            {
                SerializeGameData(null);
            }
        }
    }

    protected override void CheckGameFields()
    {
        if (backCardImage.Image.UploadedFile == null && backCardImage.Image.IsFilled == false)
        {
            ShowError("Imagem de verso da carta deve ser preenchida.", ErrorType.EMPTY, null);
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
        Debug.LogError("serialize game " + urls);
        int urlIndex = 0;
        if (backImagePath.IsNullEmptyOrWhitespace())
        {
            backImagePath = urls[0];
            urlIndex = 1;
        }

        List<Pair> listPair = new List<Pair>();
        if (filledImages.Count == pairsQtt || urls == null)
        {
            for (int i = 0; i < pairsQtt; i++)
            {
                listPair.Add(new Pair()
                {
                    firstImageUrl = filledImages[panel.idsList[i]][0],
                    secondImageUrl = filledImages[panel.idsList[i]][1]
                });
            }
        }
        else
        {
            for (int i = 0; i < pairsQtt; i++)
            {
                if (filledImages.ContainsKey(panel.idsList[i]))
                {
                    if (filledImages[panel.idsList[i]].Count == 2)
                    {
                        listPair.Add(new Pair()
                        {
                            firstImageUrl = filledImages[panel.idsList[i]][0],
                            secondImageUrl = filledImages[panel.idsList[i]][1]
                        });
                    }
                    else if (filledImages[panel.idsList[i]].Count == 1)
                    {
                        listPair.Add(
                            new Pair()
                            {
                                firstImageUrl = filledImages[panel.idsList[i]][0], secondImageUrl = urls[urlIndex]
                            });
                        urlIndex++;
                    }
                }
                else
                {
                    listPair.Add(new Pair() { firstImageUrl = urls[urlIndex], secondImageUrl = urls[urlIndex + 1] });
                    urlIndex += 2;
                }

                Debug.LogError("url index " + urlIndex + " completed pairs? " + panel.CompletedPairs());
            }
        }


        FormMatchCard completeForm = new FormMatchCard()
        {
            game = this.game,
            gameData = new MatchCard()
            {
                backImageUrl = backImagePath,
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
        FillUploadFiles(backCardImage.Image, "back_card", json.backImageUrl);
        List<string[]> urls = new List<string[]>();
        for (int i = 0; i < json.cardPairs.Count; i++)
        {
            string[] urlPair = new[] { json.cardPairs[i].firstImageUrl, json.cardPairs[i].secondImageUrl };
            urls.Add(urlPair);
        }

        panel.FillImages(urls, FillUploadFiles);
        loadFileQtt = loadFileQtt + 1 + urls.Count * 2;
        UpdateStarsPoints();
        CheckIfMaxQtt();
    }

    public void UpdateStarsPoints()
    {
        if (CalculateTimeInSec())
        {

            Debug.LogError(timeInSec );
            FillTimerText(oneStar,timeInSec * 0.3f);
            FillTimerText(twoStars,timeInSec * 0.7f);
            FillTimerText(threeStars, timeInSec);
        }
    }

    private void FillTimerText(TMP_InputField inputField, float time)
    {
        Debug.LogError("time? " +(int) time);
        int min = (int)time / 60;
        int sec = (int)time - min * 60;
        inputField.text =  String.Format("{0:00}:{1:00}", min,sec);
    }

}

[Serializable]
public class MatchCard
{
    public string backImageUrl;
    public List<Pair> cardPairs;
}

[Serializable]
public class FormMatchCard
{
    public FormBase game;
    public MatchCard gameData;
}


