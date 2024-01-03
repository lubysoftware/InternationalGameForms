using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class ImagePairJsonGet 
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
    public Nullable<int> failPenalty;
    public List<PairGet> pairs;
    public bool isDraft;
}

[Serializable]
public class ImagePairingGet
{
    public int id;
    public int gameId;
    public int failPenalty;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public List<PairGet> Pair;
}

[Serializable]
public class PairGet
{
    public int id;
    public int imageParingId;
    public string firstImageUrl;
    public string secondImageUrl;
    public string created_at;
    public string updated_at;
    public bool deleted;
}


