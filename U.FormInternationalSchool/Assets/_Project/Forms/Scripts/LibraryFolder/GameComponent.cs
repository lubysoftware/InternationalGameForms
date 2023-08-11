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
    
    private ImageSeqJsonClass game;
    private int id;

    private event Action<int, GameComponent> OnDelete;
    
    public void Init(ImageSeqJsonClass json, Action<int, GameComponent> delete)
    {
        game = json;
        id = game.id;
        gameTitle.text = json.gameTitle;
        OnDelete = delete;
    }

    private void Start()
    {
        editButton.onClick.AddListener(OnEditButton);
        deleteButton.onClick.AddListener(OnDeleteButton);
    }

    private void OnEditButton()
    {
        Debug.Log(game);
        SceneDataCarrier.AddData(Constants.GAME_EDIT, game.id);
        SceneManager.LoadScene("Form");
    }

    private void OnDeleteButton()
    {
        Debug.Log("delete");
        OnDelete?.Invoke(id, this);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
