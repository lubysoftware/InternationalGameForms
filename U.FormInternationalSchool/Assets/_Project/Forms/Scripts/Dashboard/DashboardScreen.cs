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

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GameType
{
    IMAGE_SEQ,
    IMAGE_PAIR,
    CROSS_WORDS,
    QUIZ,
    PUZZLE,
    MEMORY,
    DRAG_DROP
}
