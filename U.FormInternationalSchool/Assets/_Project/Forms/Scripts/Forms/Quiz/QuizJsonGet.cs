using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizJsonGet : MonoBehaviour
{
    public int id;
    public string gameType;
    public string gameTitle;
    public string gameTitleImageUrl;
    public string backgroundUrl;
    public string backgroundMusicUrl;
    public string questionStatementEnglishAudioUrl;
    public string questionStatementPortugueseAudioUrl;
    public string questionStatementPortugueseVersion;
    public string questionStatementEnglishVersion;
    public bool hasSupportMaterial;
    public bool hasTimer;
    public int timer;
    public int bonustimer;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public List<SupportMaterialGet> supportMaterial;
    public int quizId;
    public Nullable<int> failPenalty;
    public bool randomAnswers;
    public bool isDraft;
    public List<QuestionGet> questions;
}

public struct QuestionGet
{
    public string quizType;
    public string questionTitleEnglish;
    public string questionTitlePortuguese;
    public string questionAudioEnglishUrl;
    public string questionAudioPortugueseUrl;
    public string questionFileUrl;
    public string answerType;
    public int correctAnswer;
    public List<AnswerGet> answers;
}

public struct AnswerGet
{
    public int id;
    public int questionId;
    public string answer;
}