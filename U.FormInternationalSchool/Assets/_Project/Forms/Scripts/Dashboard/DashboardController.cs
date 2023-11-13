using System.Collections.Generic;
using UnityEngine;

public class DashboardController : MonoBehaviour
{
    [SerializeField] private List<GameTypeSO> gameSOs;
    [SerializeField] private GameTypeButton gameButton;
    [SerializeField] private Transform _buttonsHolder;

    private void Start()
    {
        DisplayButtons();
    }

    private void DisplayButtons()
    {
        foreach (GameTypeSO game in gameSOs)
        {
            GameTypeButton button = Instantiate(gameButton, _buttonsHolder);
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