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
using UnityEngine.UI;
using FileIO = System.IO.File;
using LinqUtility = Unity.VisualScripting.LinqUtility;

public class ImageSequencingForm : FormScreen
{
    [SerializeField] private ImageSequencingPanel panel;
    [SerializeField] private InputElement failsPenalty;

    private int imageSeqQtt;
    private Dictionary<int, string> filledImages;

    private int failsPenaltyValue = 0;

    private int sequenceQtt;


    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt = 1;
            FillUploadFiles(backgroundMusic, "music_theme",
                themeSongsUrls[GameType.IMAGE_SEQUENCE]);
        }
    }

    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
            FillGameData(JsonConvert.DeserializeObject<ImageSeqJsonGet>(text));
        }
    }


    protected override void SendGameFiles()
    {
        filledImages = panel.FilledImages();
        imageSeqQtt = panel.ImageQtt();
        if (imageSeqQtt < 2)
        {
            if (isPreview)
            {
                ShowError("O sequenciamento de imagens deve conter no mínimo duas imagens.", ErrorType.CUSTOM, null);
                return;
            }

            isCompleted = false;
        }
  
        if (panel.GetImages() != null && panel.GetImages().Count > 0)
        {
            SendFilesToAPI.Instance.StartUploadFiles(this, panel.GetImages(), false);
        }
        else
        {
            SerializeGameData(null);
        }
        
    }

    protected override void CheckEmptyGameFields()
    {
        if (failsPenalty.InputField.text.IsNullEmptyOrWhitespace())
        {
            failsPenalty.ActivateNullMode();
            emptyField.Add("Pontuação descontada por erro");
        }
        else
        {
            DeactivateErrorInput(failsPenalty);
        }

        if (emptyField.Count > 0)
        {
            if (isPreview)
            {
                if (emptyField.Count == 1)
                {
                    ShowError(emptyField[0], ErrorType.EMPTY, null);
                    return;
                }

                ShowError("", ErrorType.ALL_FIELDS, null);
                return;
            }

            isCompleted = false;
        }

        ValidateFields();
    }

    public void CallCheckFails()
    {
        CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro");
    }

    protected override void ValidateFields()
    {
        base.ValidateFields();
        if (hasValidationError)
        {
            return;
        }

        if (!failsPenalty.Null)
        {
            if (CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro"))
            {
                int.TryParse(failsPenalty.InputField.text, out failsPenaltyValue);
            }
            else
            {
                return;
            }
        }
        else
        {
            isCompleted = false;
        }

        if (isPreview)
        {
            if (panel.ImageQtt() < 2)
            {
                ShowError("O sequenciamento de imagens deve conter no mínimo duas imagens.", ErrorType.CUSTOM, null);
                return;
            }
        }
        
        SendBaseFormFiles();
        
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game");

        if (urls == null)
        {
            urls = filledImages.Values.ToArray();
        }
        else
        {
            previewUrlsToDelete.AddRange(urls.ToList());
        }

        List<Sequence> listSeq = new List<Sequence>();
        if (filledImages.Count == imageSeqQtt)
        {
            if (urls != null)
            {
                for (int i = 0; i < urls.Length; i++)
                {
                    listSeq.Add(new Sequence() { position = i, imageUrl = urls[i] });
                }
            }
        }
        else
        {
            int urlIndex = 0;
            for (int i = 0; i < imageSeqQtt; i++)
            {
                if (filledImages.ContainsKey(i))
                {
                    listSeq.Add(new Sequence() { position = i, imageUrl = filledImages[i] });
                }
                else
                {
                    if (urls.Length > urlIndex)
                    {
                        listSeq.Add(new Sequence() { position = i, imageUrl = urls[urlIndex] });
                        urlIndex++;
                    }
                }
            }
        }


        FormImageSequence completeForm = new FormImageSequence()
        {
            game = this.game,
            gameData = new ImageSequence()
            {
                failPenalty =  failsPenaltyValue == 0 ? null : failsPenaltyValue,
                sequences = listSeq
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
                SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this, SendGameInfoToPortal);
            }
        }
        else
        {
            FormImageSequencePreviewData preview = new FormImageSequencePreviewData()
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
                timer = timeInSec,
                questionStatementPortugueseVersion = game.questionStatementPortugueseVersion,
                failPenalty = completeForm.gameData.failPenalty,
                sequences = completeForm.gameData.sequences
            };

            ImageSequencePreview previewData = new ImageSequencePreview()
            {
                previewData = preview,
                filesToDelete = previewUrlsToDelete
            };
            
            string json = JsonConvert.SerializeObject(previewData);
            PreviewInPortal(json);
            StopLoading();
        }
    }

    private void FillGameData(ImageSeqJsonGet json)
    {
        failsPenalty.InputField.text = json.failPenalty.ToString();
        panel.FillImages(json.sequences, CheckFillFile);
        sequenceQtt = json.sequences.Count;
        CheckIfMaxQtt();
    }
    
    protected override void SetFailsPenalty(int points)
    {
        failsPenalty.InputField.text = points.ToString();
    }
}


[Serializable]
public struct Sequence
{
    public int position;
    public string imageUrl;
}

[Serializable]
public class ImageSequence
{
    public Nullable<int> failPenalty;
    public List<Sequence> sequences;
}

[Serializable]
public class FormImageSequence
{
    public FormBase game;
    public ImageSequence gameData;
}

[Serializable]
public class FormImageSequencePreviewData : BaseGameJson
{
    public Nullable<int> failPenalty;
    public List<Sequence> sequences;
}

[Serializable]
public class ImageSequencePreview
{
    public FormImageSequencePreviewData previewData;
    public List<string> filesToDelete;
}