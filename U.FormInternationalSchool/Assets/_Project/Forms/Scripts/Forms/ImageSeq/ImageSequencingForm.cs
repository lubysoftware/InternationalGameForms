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

    private string url = "image-sequence";

    private int failsPenaltyValue =0;

    private int id;

    private bool isEdit;

    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (isEdit)
        {
            SceneDataCarrier.GetData(Constants.GAME_EDIT, out id);
            SendFilesToAPI.Instance.StartDownloadGame(this, url, id);
        }
        else
        {
            StopLoading();
        }
    }
    
    public override void FinishDownloadingGame(string text)
    {
        Debug.LogError("finish");
        ImageSeqJsonGet json = JsonConvert.DeserializeObject<ImageSeqJsonGet>(text);
        Debug.LogError(json);
        if (json != null)
        {
            Debug.LogError("fill");
            FillBaseData(json);
            FillGameData(json);
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
    
    protected override void CheckGameFields()
    {
        if (failsPenalty.text.IsNullEmptyOrWhitespace())
        {
            ShowError("Pontuação descontada por erro", ErrorType.EMPTY, null);
            return;
        }

        int.TryParse(failsPenalty.text, out failsPenaltyValue);
        if (failsPenaltyValue <= 0)
        {
            ShowError("Pontuação descontada por erro", ErrorType.GREATER_THAN, new int[]{0});
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
        Debug.LogError("serialize game" + urls);

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
            SendFilesToAPI.Instance.StartUploadJsonUpdate(json, "image-sequence", id, this);
        }
        else
        {
            SendFilesToAPI.Instance.StartUploadJson(json, "image-sequence", this);
        }
    }

    protected override void FillGameData(ImageSeqJsonGet json)
    {
        failsPenalty.text = json.failPenalty.ToString();
        panel.FillImages(json.sequenceUnits, FillUploadFiles);
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
