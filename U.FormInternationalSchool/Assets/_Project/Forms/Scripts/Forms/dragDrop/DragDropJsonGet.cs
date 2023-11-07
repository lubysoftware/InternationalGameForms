using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropJsonGet : MonoBehaviour
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
    public int dragnDropId;
    public string dropPlaceBackgroundUrl;
    public string materialType;
    public string dragType;
    public int failPenalty;
    public List<DraggableItemJson> draggableItem;
}

[Serializable]
public class DragItemGet
{
    public int id;
    public int dragndropId;
    public int groupId;
    public float spawnPointX;
    public float spawnPointY;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public string imageUrl;
}

