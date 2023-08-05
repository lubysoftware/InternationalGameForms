using System;
using System.Collections;
using System.Collections.Generic;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using FileIO = System.IO.File;
public class ImageSequencingForm : FormScreen
{
    [SerializeField] private ImageSequencingPanel panel;
    [SerializeField] private TMP_InputField failsPenalty;
    private readonly string PATH = Application.dataPath + "/Teste.json";

    private int failsPenaltyValue =0;

    
    protected override void SendGameFiles()
    {
        if (panel.GetImages() != null)
        {
            SendFilesToAPI.Instance.StartUploadFiles(panel.GetImages());
        }
        else
        {
            ShowError("O sequenciamento de imagens deve conter no mínimo duas imagens.", ErrorType.CUSTOM, null);
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

        if (panel.GetImages() == null || panel.GetImages().Count < 2)
        {
            ShowError("O sequenciamento de imagens deve conter no mínimo duas imagens.", ErrorType.CUSTOM, null);
            return;
        }
        
        SendBaseFormFiles();
    }

    protected override void SerializeGameData(string[] urls)
    {
        List<Sequence> listSeq = new List<Sequence>();
        if (urls != null)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                listSeq.Add(new Sequence(){ position = i, imageUrl = urls[i]});
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

        SendFilesToAPI.Instance.StartUploadJson(json, "https://school.gamehub.api.oke.luby.me/image-sequence");
           
        FileIO.WriteAllText(PATH, json);
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
