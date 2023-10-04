using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using FileIO = System.IO.File;

public class ImageSequencingForm : FormScreen
{
    [SerializeField] private ImageSequencingPanel panel;
    [SerializeField] private TMP_InputField failsPenalty;

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
            loadFileQtt=1;
            FillUploadFiles( backgroundMusic,"music_theme","https://stg1atividades.blob.core.windows.net/arquivos/0c917c36-1e93-489a-a4d0-e4327cffc752_name.001_img_sequencing.ogg");
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
            ShowError("O sequenciamento de imagens deve conter no mínimo duas imagens.", ErrorType.CUSTOM, null);
        }
        else
        {
            if (panel.GetImages() != null && panel.GetImages().Count > 0)
            {
                SendFilesToAPI.Instance.StartUploadFiles(this, panel.GetImages(), false);
            }else
            {
                SerializeGameData(filledImages.Values.ToArray());
            }
        }
    }
    
    protected override void CheckEmptyGameFields()
    {
        if (failsPenalty.text.IsNullEmptyOrWhitespace())
        {
            SetInputColor(failsPenalty, true);
            hasEmptyError = true;
        }else
        {
            SetInputColor(failsPenalty, false);
        }

        if (hasEmptyError)
        {
            ShowError("", ErrorType.ALL_FIELDS, null);
            return;
        }
        
        ValidateFields();
    }
    
    public void CallCheckFails()
    {
        CheckFailsPenalty(failsPenalty);
    }
    
    protected override void ValidateFields()
    {
        base.ValidateFields();
        if (hasValidationError)
        {
            return;
        }
        if (CheckFailsPenalty(failsPenalty))
        {
            int.TryParse(failsPenalty.text, out failsPenaltyValue);
        }
        else
        {
            return;
        }

        if (panel.ImageQtt() < 2)
        {
            ShowError("O sequenciamento de imagens deve conter no mínimo duas imagens.", ErrorType.CUSTOM, null);
            return;
        }
        
        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game" + urls);

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
                    listSeq.Add(new Sequence(){ position = i, imageUrl = filledImages[i]});
                }
                else
                {
                    if (urls.Length > urlIndex)
                    {
                        listSeq.Add(new Sequence(){ position = i, imageUrl = urls[urlIndex]});
                        urlIndex++;
                    }
                }
            }
        }
       

        FormImageSequence completeForm = new FormImageSequence()
        {
            game = this.game,
            gameData =  new ImageSequence()
            {
                failPenalty = failsPenaltyValue,
                sequences = listSeq
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

    private void FillGameData(ImageSeqJsonGet json)
    {
        failsPenalty.text = json.failPenalty.ToString();
        panel.FillImages(json.sequenceUnits, FillUploadFiles);
        sequenceQtt = json.sequenceUnits.Count;
        loadFileQtt = loadFileQtt + sequenceQtt;
        CheckIfMaxQtt();
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
    public int failPenalty;
    public List<Sequence> sequences;
}

[Serializable]
public class FormImageSequence
{
    public FormBase game;
    public ImageSequence gameData;
}
