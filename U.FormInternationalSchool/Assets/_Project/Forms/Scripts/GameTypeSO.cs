using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameTypeSO")]
[Serializable]
public class GameTypeSO : ScriptableObject
{
   public GameType gameType;
   public string title;
   public Sprite sprite;
   public string description;
   public string url;
   public bool isActive;
   public Color[] colors;
   public string scene;
}
