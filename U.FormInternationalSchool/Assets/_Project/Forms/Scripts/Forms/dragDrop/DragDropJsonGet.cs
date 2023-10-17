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
    public List<SupportMaterialGet> SupportMaterial;
    public int dragnDropId;
    public string dropPlaceBackgroundUrl;
    public List<DraggableItem> draggableItem;
}
    
[Serializable]
public class DragDropGet
{
    public int id;
    public int matchCardId;
    public string firstImageUrl;
    public string secondImageUrl;
    public string created_at;
    public string updated_at;
    public bool deleted;
}

[Serializable]
public class DragItemGet
{
    public int id;
    public int dragndropId;
    public string dragType;
    public float spawnPointX;
    public float spawnPointY;
    public string created_at;
    public string updated_at;
    public bool deleted;
    public string imageUrl;
}

