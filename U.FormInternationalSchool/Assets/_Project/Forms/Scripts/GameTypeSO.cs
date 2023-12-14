using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

[CreateAssetMenu(menuName = "SO/GameTypeSO")]
[Serializable]
public class GameTypeSO : ScriptableObject
{
   public GameType gameType;
   public string title;
   public Sprite sprite;
   public Sprite headerSprite;
   public string description;
   public string url;
   public bool isActive;
   public Color[] colors;
   public string scene;
   public Sprite popup;
}
