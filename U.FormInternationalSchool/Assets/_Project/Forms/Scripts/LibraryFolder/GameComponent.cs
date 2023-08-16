using System;
using System.Collections;
using System.Collections.Generic;
using LubyLib.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameComponent : MonoBehaviour
{
    [SerializeField] private Button editButton;

    [SerializeField] private Button deleteButton;

    [SerializeField] private TextMeshProUGUI gameTitle;

    [SerializeField] private LibraryScreen library;
    
    private ImageSeqJsonClass game;
    private int id;

    public void Init(ImageSeqJsonClass json)
    {
        game = json;
        id = game.id;
        gameTitle.text = json.gameTitle;
    }

    private void Start()
    {
        editButton.onClick.AddListener(OnEditButton);
        deleteButton.onClick.AddListener(OnDeleteButton);
    }

    private void OnEditButton()
    {
        SceneDataCarrier.AddData(Constants.IS_EDIT, true);
        SceneDataCarrier.AddData(Constants.GAME_EDIT, game.id);
        SceneManager.LoadScene("Form");
    }

    private void OnDeleteButton()
    {
        library.OnDeleteGame(game.id,this,game.gameTitle );
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
