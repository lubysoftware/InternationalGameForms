using System;
using System.Collections;
using System.Collections.Generic;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class QuizForm : FormScreen
{
    [SerializeField] private QuestionsGroup questionsGroup;
    [SerializeField] private InputElement failsPenalty;
    [SerializeField] private Toggle randomize;
    
    private int failsPenaltyValue = 0;
    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt = 1;
            FillUploadFiles(backgroundMusic, "music_theme",
                "https://stg1atividades.blob.core.windows.net/arquivos/3ddfb2ca-57b5-4489-b4cd-7c1c2ff3d7f1_name.003_quiz.ogg");
        }
    }

    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
            FillGameData(JsonConvert.DeserializeObject<QuizJsonGet>(text));
        }
    }


    protected override void SendGameFiles()
    {
        questionsGroup.GetAllQuestionData();
    }

    protected override void CheckEmptyGameFields()
    {
        if (failsPenalty.InputField.text.IsNullEmptyOrWhitespace())
        {
            failsPenalty.ActivateErrorMode();
            emptyField.Add("Pontuação descontada por erro");
        }
        else
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
        
        if (!questionsGroup.CheckAllQuestions())
        {
            string errormessage = "Todos as questões devem ser preenchidas.";
            ShowError(errormessage, ErrorType.CUSTOM, null);
            return;
        }

        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
    }

    public void SerializeGameData(Question[] questionsList)
    {
        FormQuiz completeForm = new FormQuiz()
        {
            game = this.game,
            gameData = new Quiz()
            {
                failPenalty = failsPenaltyValue,
                randomAnswers = randomize.isOn,
                questions = questionsList,
            }
        };


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


    private void FillGameData(QuizJsonGet json)
    {
        failsPenalty.InputField.text = json.failPenalty.ToString();
        randomize.SetIsOnWithoutNotify(json.randomAnswers);
        questionsGroup.FillQuestions(json.questions.ToArray());
        CheckIfMaxQtt();
    }
}

[Serializable]
public class FormQuiz
{
    public FormBase game;
    public Quiz gameData;
}


