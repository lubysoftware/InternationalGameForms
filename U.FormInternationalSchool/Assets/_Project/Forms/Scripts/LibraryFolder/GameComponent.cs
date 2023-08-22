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
    

    private string title;
    private int id;
    private string scene;

    public void Init(string titleGame, int idGame, string sceneGame)
    {
        title = titleGame;
        id = idGame;
        scene = sceneGame;
        gameTitle.text = title;
    }

    private void Start()
    {
        editButton.onClick.AddListener(OnEditButton);
        deleteButton.onClick.AddListener(OnDeleteButton);
    }

    private void OnEditButton()
    {
        SceneDataCarrier.AddData(Constants.IS_EDIT, true);
        SceneDataCarrier.AddData(Constants.GAME_EDIT, id);
        SceneManager.LoadScene(scene);
    }

    private void OnDeleteButton()
    {
        library.OnDeleteGame(id,this,title );
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
