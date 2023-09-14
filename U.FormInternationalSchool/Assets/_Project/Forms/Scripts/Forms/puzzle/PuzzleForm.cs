using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PuzzleForm : FormScreen
{
    [SerializeField] private TMP_Dropdown tipQtt;
    [SerializeField] private TMP_Dropdown piecesQtt;
    [SerializeField] private UploadFileElement puzzleImage;
    [SerializeField] private TextMeshProUGUI oneStar;
    [SerializeField] private TextMeshProUGUI twoStars;
    [SerializeField] private TextMeshProUGUI threeStars;
    [SerializeField] private Toggle tips;
    [SerializeField] protected TMP_InputField timeCooldownMin, timeCooldownSec;
    [SerializeField] protected TMP_InputField timeTipMin, timeTipSec;

    private int tipTimeInSec = 0;
    private int cooldownTimeInSec = 0;

    private string puzzleImagePath = "";

    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
            FillGameData(JsonConvert.DeserializeObject<PuzzleJsonGet>(text));
        }
    }


    protected override void SendGameFiles()
    {
        List<File> files = new List<File>();
        if (puzzleImage.UploadedFile != null)
        {
            files.Add(puzzleImage.UploadedFile);
            puzzleImagePath = "";
            SendFilesToAPI.Instance.StartUploadFiles(this, files, false);
        }
        else
        {
            if (puzzleImage.IsFilled)
            {
                SerializeGameData(null);
                puzzleImagePath = puzzleImage.url;
            }
            else
            {
                ShowError("Imagem do quebra-cabeça.", ErrorType.EMPTY, null);
            }
        }
    }

    protected override void CheckGameFields()
    {
        if (puzzleImage.UploadedFile == null && puzzleImage.IsFilled == false)
        {
            ShowError("Imagem do quebra-cabeça.", ErrorType.EMPTY, null);
            return;
        }

        if (tips.isOn)
        {
            if (timeTipMin.text.IsNullEmptyOrWhitespace())
            {
                ShowError("Minutos do timer de dicas", ErrorType.EMPTY, null);
                return;
            }

            tipTimeInSec = CalculateTimeInSec("de dicas", timeTipMin.text, timeTipSec.text, false);
            if (timeInSec == -1)
            {
                tipTimeInSec = 0;
                return;
            }

            if (tipTimeInSec > timeInSec)
            {
                ShowError("O timer de dicas não deve possuir um tempo maior que o tempo total do jogo.", ErrorType.CUSTOM, null);
                return;
            }

            cooldownTimeInSec = CalculateTimeInSec("de intervalo de dicas", timeCooldownMin.text, timeCooldownSec.text, true);
            if (timeInSec == -1)
            {
                cooldownTimeInSec = 0;
                return;
            }
        }
        else
        {
            tipTimeInSec = 0;
            cooldownTimeInSec = 0;
        }

        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.LogError("serialize game " + urls);

        if (puzzleImagePath.IsNullEmptyOrWhitespace())
        {
            puzzleImagePath = urls[0];
        }
        
        int pieces = 0;
        int.TryParse(piecesQtt.options[piecesQtt.value].text, out pieces);
        int tips = 0;
        if (this.tips.isOn)
        {
            int.TryParse(tipQtt.options[tipQtt.value].text, out tips);
        }
        FormPuzzle completeForm = new FormPuzzle()
        {
            game = this.game,
            gameData = new Puzzle()
            {
                imageUrl = puzzleImagePath,
                pieceCount = pieces,
                tipCoolDown = this.tips.isOn? cooldownTimeInSec:0,
                tipShowTime = this.tips.isOn? tipTimeInSec:0,
                tipCount = this.tips.isOn? tips : 0
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

    private void FillGameData(PuzzleJsonGet json)
    {
        FillUploadFiles(puzzleImage, "puzzle_image", json.imageUrl);
        piecesQtt.value = piecesQtt.options.FindIndex(x => x.text == json.pieceCount.ToString());
        bool hasTips = json.tipCount > 0;
        tips.SetIsOnWithoutNotify(hasTips);
        if (hasTips)
        {
            tipQtt.value = tipQtt.options.FindIndex(x => x.text == json.tipCount.ToString());
            tips.onValueChanged.Invoke(hasTips);
            int min = json.tipShowTime / 60;
            int sec = json.tipShowTime - min * 60;
            timeTipMin.text = String.Format("{0:00}", min);
            timeTipSec.text = String.Format("{0:00}", sec);

            int coolMin = 0;
            int coolSec = 0;
            if (json.tipCoolDown > 0)
            {
                coolMin = json.tipShowTime / 60;
                coolSec = json.tipShowTime - min * 60;
            }
            timeTipMin.text =  String.Format("{0:00}", coolMin);
            timeTipSec.text = String.Format("{0:00}", coolSec);
        }
        
        loadFileQtt = loadFileQtt + 1;
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
public class Puzzle
{
    public string imageUrl;
    public int pieceCount;
    public int tipShowTime;
    public int tipCoolDown;
    public int tipCount;
}

[Serializable]
public class FormPuzzle
{
    public FormBase game;
    public Puzzle gameData;
}


