using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class ImagePairingForm : FormScreen
{
    [SerializeField] private ImagePairPanel panel;
    [SerializeField] private InputElement failsPenalty;
    private int pairsQtt;
    private Dictionary<char,List<string>> filledImages;
    private int failsPenaltyValue = 0;
    
    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt=1;
            FillUploadFiles(backgroundMusic,"music_theme","https://stg1atividades.blob.core.windows.net/arquivos/1e793b75-8a67-4dbd-8bf8-38ffc8e99beb_name.002_img_pairing.ogg");
        }
    }
    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
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
    
    protected override void ValidateFields()
    {
        base.ValidateFields();
        if (hasValidationError)
        {
            return;
        }
        if (CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro"))
        {
            int.TryParse(failsPenalty.InputField.text, out failsPenaltyValue);
        }
        else
        {
            return;
        }
        
        if (!panel.AllPairsFilled())
        {
            ShowError("Todos os pares devem ser preenchidos.", ErrorType.CUSTOM, null);
            return;
        }
        
        SendBaseFormFiles();
    }
    
    protected override void CheckEmptyGameFields()
    {
        if (failsPenalty.InputField.text.IsNullEmptyOrWhitespace())
        {
            failsPenalty.ActivateErrorMode();
            emptyField.Add("Pontuação descontada por erro");
        }else
        {
            DeactivateErrorInput(failsPenalty);
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
    
    public void CallCheckFails()
    {
        CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro");
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game " + urls);

        if (urls != null)
        {
            urlsToDelete.AddRange(urls.ToList());
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
            int urlIndex = 0;
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


        FormImagePairing completeForm = new FormImagePairing()
        {
            game = this.game,
            gameData = new ImagePairing()
            {
                failPenalty = failsPenaltyValue,
                pairs = listPair
            }
        };

        if (!isPreview)
        {
            string json = JsonConvert.SerializeObject(completeForm);
            if (isEdit)
            {
                SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.InputField.text, this, SendGameInfoToPortal);
            }
            else
            {
                SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this, SendGameInfoToPortal);
            }
        }
        else
        {
            FormImagePairingPreviewData preview = new FormImagePairingPreviewData()
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
                failPenalty = completeForm.gameData.failPenalty,
                pairs = completeForm.gameData.pairs
            };

            ImagePairingPreview previewData = new ImagePairingPreview()
            {
                previewData = preview,
                filesToDelete = urlsToDelete
            };
            
            string json = JsonConvert.SerializeObject(previewData);
            PreviewInPortal(json);
            StopLoading();
        }
        
    }
    
 

    private void FillGameData(ImagePairJsonGet json)
    {
        failsPenalty.InputField.text = json.failPenalty.ToString();
        List<string[]> urls = new List<string[]>();
        for (int i = 0; i < json.pairs.Count; i++)
        {
            string[] urlPair = new[] { json.pairs[i].firstImageUrl, json.pairs[i].secondImageUrl };
            urls.Add(urlPair);
        }
        panel.FillImages(urls, FillUploadFiles);
        loadFileQtt = loadFileQtt + urls.Count * 2;
        CheckIfMaxQtt();
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


[Serializable]
public class FormImagePairingPreviewData : BaseGameJson
{
    public int failPenalty;
    public List<Pair> pairs;
}

[Serializable]
public class ImagePairingPreview
{
    public FormImagePairingPreviewData previewData;
    public List<string> filesToDelete;
}

