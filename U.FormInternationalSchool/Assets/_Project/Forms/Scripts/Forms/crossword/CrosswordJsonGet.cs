using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CrosswordJsonGet
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
    public Nullable<int> timer;
    public int bonustimer;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public List<SupportMaterialGet> supportMaterial; 
    public int crossWordId;
    public string questionType;
    public List<WordsGet> words;
    public bool isDraft;
}

[Serializable]
public class WordsGet
{
    public int id;
    public int crossWordId;
    public string question;
    public string answer;
    public string orientation;
    public int posX;
    public int posY;
    public string created_at;
    public string updated_at;
    public bool deleted;
}
