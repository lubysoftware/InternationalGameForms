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

        if (questionsGroup.QuestionsQtt == 0)
        { 
            string errormessage = "O quiz deve possuir ao menos uma questão.";
            ShowError(errormessage, ErrorType.CUSTOM, null);
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

    public void SerializeGameData(Question[] questionsList, List<string> deleteUrls)
    {
        
        urlsToDelete.AddRange(deleteUrls);
        
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
            FormQuizPreviewData preview = new FormQuizPreviewData()
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
                randomAnswers = completeForm.gameData.randomAnswers,
                questions = completeForm.gameData.questions
            };

            QuizPreview previewData = new QuizPreview()
            {
                previewData = preview,
                filesToDelete = urlsToDelete
            };
            
            string json = JsonConvert.SerializeObject(previewData);
            PreviewInPortal(json);
            StopLoading();
        }
    }


    private void FillGameData(QuizJsonGet json)
    {
        failsPenalty.InputField.text = json.failPenalty.ToString();
        randomize.SetIsOnWithoutNotify(json.randomAnswers);
        questionsGroup.FillQuestions(json.questions.ToArray());
        CheckIfMaxQtt();
    }
    
    public void UpdatePoints()
    {
        failsPenalty.InputField.text = questionsGroup.QuestionsQtt > 0 ? (100/questionsGroup.QuestionsQtt).ToString():0.ToString();
    }
}

[Serializable]
public class FormQuiz
{
    public FormBase game;
    public Quiz gameData;
}

public class FormQuizPreviewData : BaseGameJson
{
    public int failPenalty;
    public bool randomAnswers;
    public Question[] questions;
}

public class QuizPreview
{
    public FormQuizPreviewData previewData;
    public List<string> filesToDelete;
}


