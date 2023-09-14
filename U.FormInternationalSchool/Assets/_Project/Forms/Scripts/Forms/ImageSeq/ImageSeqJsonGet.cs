using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImageSeqJsonGet
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
    public int imageSequenceId;
    public int failPenalty;
    public List<SequenceGet> sequenceUnits;
}


[Serializable]
public class ImageSequenceGet
{
    public int id;
    public int gameId;
    public int failPenalty;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public List<SequenceGet> Sequence;
}
[Serializable]
public class SequenceGet
{
    public int id;
    public int imageSequenceId;
    public int position;
    public string imageUrl;
    public string created_at;
    public string updated_at;
    public bool deleted;
}
