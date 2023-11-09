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
    [SerializeField] protected InputElement timeCooldownMin, timeCooldownSec;
    [SerializeField] protected InputElement timeTipMin, timeTipSec;

    private int tipTimeInSec = 0;
    private int cooldownTimeInSec = 0;

    private string puzzleImagePath ;


    
    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt=1;
            FillUploadFiles( backgroundMusic,"music_theme","https://stg1atividades.blob.core.windows.net/arquivos/3cd23a4c-f710-454e-9a2f-7244cadbadc7_name.006_puzzle.ogg");
        }
    }
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
                puzzleImagePath = puzzleImage.url;
                SerializeGameData(null);
            }
            else
            {
                ShowError("Imagem do quebra-cabeça.", ErrorType.EMPTY, null);
            }
        }
    }

    protected override void CheckEmptyGameFields()
    {
        if (puzzleImage.UploadedFile == null && puzzleImage.IsFilled == false)
        {
            puzzleImage.ActivateErrorMode();
            emptyField.Add("Imagem do quebra cabeça");
        }

        if (tips.isOn)
        {
            if (timeTipMin.InputField.text.IsNullEmptyOrWhitespace())
            {
                timeTipMin.ActivateErrorMode();
                emptyField.Add("Minutos do timer de dicas");
            }
            if (timeTipSec.InputField.text.IsNullEmptyOrWhitespace())
            {
                timeTipSec.ActivateErrorMode();
                emptyField.Add("Segundos do timer de dicas");
            }
        }
        else
        {
            tipTimeInSec = 0;
            cooldownTimeInSec = 0;
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

    protected override void ValidateFields()
    {
        base.ValidateFields();
        if (hasValidationError) return;
        
        if (tips.isOn)
        {
            tipTimeInSec = CalculateTimeInSec("de dicas", timeTipMin, timeTipSec, false);
            if (tipTimeInSec == -1)
            {
                tipTimeInSec = 0;
                return;
            }

            if (tipTimeInSec > timeInSec)
            {
                timeTipMin.ActivateErrorMode();
                ShowError("O timer de dicas não deve possuir um tempo maior que o tempo total do jogo.", ErrorType.CUSTOM, null);
                return;
            }

            cooldownTimeInSec = CalculateTimeInSec("de intervalo", timeCooldownMin, timeCooldownSec, true);
            if (cooldownTimeInSec == -1)
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
        Debug.Log("serialize game " + urls);

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
        Debug.Log(json);
        if (isEdit)
        {
            SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.InputField.text, this, SendGameInfoToPortal);
        }
        else
        {
            SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this, SendGameInfoToPortal);
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
            timeTipMin.InputField.text = String.Format("{0:00}", min);
            timeTipSec.InputField.text = String.Format("{0:00}", sec);

            int coolMin = 0;
            int coolSec = 0;
            if (json.tipCoolDown > 0)
            {
                coolMin = json.tipCoolDown / 60;
                coolSec = json.tipCoolDown - coolMin * 60;
            }
            timeCooldownMin.InputField.text =  String.Format("{0:00}", coolMin);
            timeCooldownSec.InputField.text = String.Format("{0:00}", coolSec);
        }
        
        loadFileQtt = loadFileQtt + 1;
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


