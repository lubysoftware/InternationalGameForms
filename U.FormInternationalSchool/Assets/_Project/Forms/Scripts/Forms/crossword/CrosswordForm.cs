using System;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using FileIO = System.IO.File;

public class CrosswordForm : FormScreen
{
    [SerializeField] private CrosswordPanel panel;
    [SerializeField] private InputElement pointPerWord;

    private int wordsQtt;
    private Dictionary<NewWordInput.WordInfo, string> filledImages;
    

    private int wordImagesQtt;

    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt=1;
            FillUploadFiles( backgroundMusic,"music_theme","https://stg1atividades.blob.core.windows.net/arquivos/97bf793e-3497-44e5-a698-5890314692db_name.005_crosswords.ogg");
        }
    }
    
    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
            FillGameData(JsonConvert.DeserializeObject<CrosswordJsonGet>(text));
        }
    }
    
    protected override void SendGameFiles()
    {
        Debug.Log("send game files");
        wordsQtt = panel.WordsQtt;
        if (wordsQtt < 1)
        {
            ShowError("O grid de palavras deve conter no mínimo uma palavra.", ErrorType.CUSTOM, null);
        }
        else
        {
            if (panel.IsImage)
            {
                filledImages = panel.FilledImage();
                if (panel.GetImages() != null && panel.GetImages().Count > 0)
                {
                    SendFilesToAPI.Instance.StartUploadFiles(this, panel.GetImages(), false);
                }
                else
                {
                    SerializeGameData(filledImages.Values.ToArray());
                }
            }
            else
            {
                filledImages = panel.FilledInputs();
                SerializeGameData(filledImages.Values.ToArray());
            }
        }
    }
    
    protected override void CheckEmptyGameFields()
    {
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
        if (panel.WordsQtt < 1)
        {
            ShowError("O grid de palavras deve conter no mínimo uma palavra.", ErrorType.CUSTOM, null);
            return;
        }

        if (panel.editingInput != null)
        {
            ShowError("Finalize a edição da palavra "+ panel.editingInput.Index+".", ErrorType.CUSTOM, null);
            return;
        }

        if (!panel.IsAllFilled())
        {
            ShowError("Todas as palavras devem ter as dicas preenchidas.", ErrorType.CUSTOM, null);
            return;
        }
        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game" + urls.Length);
        List<Words> listWords = new List<Words>();
        if (filledImages.Count == wordsQtt)
        {
            foreach(var input in filledImages) 
            {
                int row = WordsGrid.idsRow.ToList().FindIndex(x => x == input.Key.Coord.row);
                listWords.Add(new Words() { question = input.Value, answer = input.Key.Word, posY = input.Key.Coord.column - 1, posX = row, orientation = input.Key.IsHorizontal? "HORIZONTAL": "VERTICAL"});
            }
        }
        else
        {
            int urlIndex = 0;
            foreach (var input in panel.GetWordInputs())
            {
                int row = WordsGrid.idsRow.ToList().FindIndex(x => x == input.Info.Coord.row);
                if (filledImages.ContainsKey(input.Info))
                {
                    listWords.Add(new Words() { question = filledImages[input.Info], answer = input.Info.Word, posY = input.Info.Coord.column -1, posX = row, orientation = input.Info.IsHorizontal? "HORIZONTAL": "VERTICAL"});
                }
                else
                {
                    if (urls.Length > urlIndex)
                    {
                        listWords.Add(new Words() { question = urls[urlIndex], answer = input.Info.Word, posY = input.Info.Coord.column -1, posX = row, orientation = input.Info.IsHorizontal? "HORIZONTAL": "VERTICAL"});
                        urlIndex++;
                    }
                }
            }
        }
        
        FormCrossword completeForm = new FormCrossword()
        {
            game = this.game,
            gameData =  new Crossword()
            {
                questionType = panel.IsImage? "IMAGE":"TEXT",
                words = listWords
            }
        };

       
        string json = JsonConvert.SerializeObject(completeForm);
        if (isEdit)
        {
            SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.InputField.text, this);
        }
        else
        {
            SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this);
        }
    }

    private void FillGameData(CrosswordJsonGet json)
    {
        pointPerWord.InputField.text = (100/json.words.Count).ToString();
        bool isImage = json.questionType == "IMAGE";
        panel.SetImageToggle(isImage);
        panel.FillData(json.words,FillUploadFiles,isImage);
        
        wordImagesQtt = isImage?json.words.Count:0;
        loadFileQtt = loadFileQtt + wordImagesQtt;
        CheckIfMaxQtt();
    }

    public void UpdatePoints()
    {
        pointPerWord.InputField.text = panel.WordsQtt > 0 ? (100/panel.WordsQtt).ToString():0.ToString();
    }
}


[Serializable]
public struct Words
{
    public string question;
    public string answer;
    public string orientation;
    public int posX;
    public int posY;
}

[Serializable]
public class Crossword
{
    public string questionType;
    public List<Words> words;
}

[Serializable]
public class FormCrossword
{
    public FormBase game;
    public Crossword gameData;
}
