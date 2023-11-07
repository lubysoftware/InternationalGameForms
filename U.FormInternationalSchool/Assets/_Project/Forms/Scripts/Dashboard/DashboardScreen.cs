using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DashboardScreen : MonoBehaviour
{
    [SerializeField] private List<GameTypeSO> gameSOs;
    [SerializeField] private GameTypeButton gameButton;
    void Start()
    {
        foreach (GameTypeSO game in gameSOs)
        {

            GameTypeButton button = Instantiate(gameButton, this.transform);
            button.Init(game);
            
            
        } 
    }
    
}

public enum GameType
{
    IMAGE_SEQUENCE,
    IMAGE_PARING,
    CROSS_WORD,
    QUIZ,
    PUZZLE,
    MATCH_CARD,
    DRAGNDROP
}
