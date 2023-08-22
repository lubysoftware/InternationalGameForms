using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImageSeqJsonClass
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
    public List<ImageSequenceGet> ImageSequence;
}



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
public class BaseJsonGet
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

public class Meta
{
    public int page;
    public int perPage;
    public int countItems;
    public int lastPage;
}

public class ImageSeqList
{
    public Meta meta;
    public List<ImageSeqJsonClass> data;
}