using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            loadFileQtt = 2;
            FillUploadFiles(backgroundMusic, "music_theme",
                themeSongsUrls[GameType.MATCH_CARD]);
            FillUploadFiles(backCardImage.Image, "back_card",
                "https://stg1atividades.blob.core.windows.net/arquivos/f8e9e553-ccc2-4a48-b9b0-3c205d73357d_name.verso.png");
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

    protected override void CheckEmptyGameFields()
    {
        if (backCardImage.Image.UploadedFile == null && backCardImage.Image.IsFilled == false)
        {
            backCardImage.GetComponentInChildren<InputElement>().ActivateErrorMode();
            emptyField.Add("Imagem de verso da carta");
        }

        if (emptyField.Count > 0)
        {
            if (emptyField.Count == 1)
            {
                ShowError(emptyField[0], ErrorType.EMPTY, null);
                return;
            }

            ShowError("", ErrorType.ALL_FIELDS, null);
            return;
        }

        ValidateFields();
    }

    protected virtual void ValidateFields()
    {
        base.ValidateFields();
        if (hasValidationError) return;
        if (!panel.AllPairsFilled())
        {
            ShowError("Todos os pares devem ser preenchidos.", ErrorType.CUSTOM, null);
            return;
        }

        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game " + urls);

        if (urls != null)
        {
            previewUrlsToDelete.AddRange(urls.ToList());
        }

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


        if (!isPreview)
        {
            string json = JsonConvert.SerializeObject(completeForm);

            if (isEdit)
            {
                SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.InputField.text, this,
                    SendGameInfoToPortal);
            }
            else
            {
                SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this,
                    SendGameInfoToPortal);
            }
        }
        else
        {
            FormMatchCardPreviewData preview = new FormMatchCardPreviewData()
            {
                gameTitle = game.gameTitle,
                backgroundMusicUrl = game.backgroundMusicUrl,
                backgroundUrl = game.backgroundUrl,
                bonustimer = game.bonustimer,
                gameTitleImageUrl = game.gameTitleImageUrl,
                hasSupportMaterial = game.hasSupportMaterial,
                SupportMaterial = game.supportMaterial,
                hasTimer = game.hasTimer,
                questionStatementEnglishAudioUrl = game.questionStatementEnglishAudioUrl,
                questionStatementEnglishVersion = game.questionStatementEnglishVersion,
                questionStatementPortugueseAudioUrl = game.questionStatementPortugueseAudioUrl,
                timer = game.timer,
                questionStatementPortugueseVersion = game.questionStatementPortugueseVersion,
                backImageUrl = completeForm.gameData.backImageUrl,
                cardPairs = completeForm.gameData.cardPairs
            };

            MatchCardPreview previewData = new MatchCardPreview()
            {
                previewData = preview,
                filesToDelete = previewUrlsToDelete
            };
            
            string json = JsonConvert.SerializeObject(previewData);
            PreviewInPortal(json);
            StopLoading();
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
        int time = CalculateTimeInSec("do jogo", timeMin, timeSec, false);
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

[Serializable]
public class FormMatchCardPreviewData : BaseGameJson
{
    public string backImageUrl;
    public List<Pair> cardPairs;
}

[Serializable]
public class MatchCardPreview
{
    public FormMatchCardPreviewData previewData;
    public List<string> filesToDelete;
}


