using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MemoryJsonClass
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
    public int bonusTimer;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public List<SupportMaterialGet> SupportMaterial;
}

[Serializable]
public class MemoryJsonGet 
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
    public List<SupportMaterialGet> SupportMaterial;
    public int matchCardId;
    public string backImageUrl;
    public List<MatchPairGet> cardPairs;
}

[Serializable]
public class MatchPairGet
{
    public int id;
    public int matchCardId;
    public string firstImageUrl;
    public string secondImageUrl;
    public string created_at;
    public string updated_at;
    public bool deleted;
}

public class MatchCardList
{
    public Meta meta;
    public List<ImagePairJsonClass> data;
}

