using System.Collections.Generic;
using UnityEngine;

public class DashboardScreen : MonoBehaviour
{
    [SerializeField] private List<GameTypeSO> gameSOs;
    [SerializeField] private GameTypeButton gameButton;
    [SerializeField] private Transform _gamesHolder;

    private void Start()
    {
        foreach (GameTypeSO game in gameSOs)
        {
            GameTypeButton button = Instantiate(gameButton, _gamesHolder);
            button.Init(game);
        }
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