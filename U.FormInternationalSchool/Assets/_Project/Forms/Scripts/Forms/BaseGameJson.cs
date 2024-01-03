using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BaseGameJson
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
    public List<SupportMaterial> SupportMaterial;
    public bool isDraft;
}


[Serializable]
public class SupportMaterialGet
{
    public int id;
    public int gameId;
    public int position;
    public string material;
    public string materialType;
    public string created_at;
    public string updated_at;
    public bool deleted;
}
public class GameList
{
    public Meta meta;
    public List<BaseGameJson> data;
}

public class Meta
{
    public int page;
    public int perPage;
    public int countItems;
    public int lastPage;
}


