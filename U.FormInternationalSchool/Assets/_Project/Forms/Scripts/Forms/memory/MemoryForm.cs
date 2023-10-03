using System;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class MemoryForm : FormScreen
{
    [SerializeField] private MemoryPanel panel;
    private int pairsQtt;
    private Dictionary<char, List<string>> filledImages;
    [SerializeField] private ImageFrame backCardImage;
    [SerializeField] private TextMeshProUGUI oneStar;
    [SerializeField] private TextMeshProUGUI twoStars;
    [SerializeField] private TextMeshProUGUI threeStars;

    private string backImagePath = "";


    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt=2;
            FillUploadFiles( backgroundMusic,"music_theme","https://stg1atividades.blob.core.windows.net/arquivos/6c3eafb6-76af-4cd4-a467-c3edcaf68161_name.007_memory.ogg");
            FillUploadFiles( backCardImage.Image,"back_card","https://stg1atividades.blob.core.windows.net/arquivos/e708b2c7-725c-45ee-be93-79ac9ef04b47_name.verso.png");
        }
    }

    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
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
            ShowError("Imagem de verso da carta", ErrorType.EMPTY, null);
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
        if (urls == null)
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
        Debug.Log(json);
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
        int time = CalculateTimeInSec("do jogo", timeMin.text, timeSec.text, false);
        if (time >= 0)
        {
            timeInSec = time;
            FillTimerText(oneStar,timeInSec * 0.3f);
            FillTimerText(twoStars,timeInSec * 0.7f);
            FillTimerText(threeStars, timeInSec);
        }
    }

    private void FillTimerText(TextMeshProUGUI field, float time)
    {
        int hour = 0;
        int min = (int)time / 60;
        int sec = (int)time - min * 60;
        if (min >= 60)
        {
            hour = min / 60;
            min = min - hour * 60;
        }
        if (hour > 0)
        {
            field.text =  String.Format("{0:00}:{1:00}:{2:00}", hour,min,sec);
        }
        else
        {
            field.text =  String.Format("{0:00}:{1:00}", min,sec);
        }
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


