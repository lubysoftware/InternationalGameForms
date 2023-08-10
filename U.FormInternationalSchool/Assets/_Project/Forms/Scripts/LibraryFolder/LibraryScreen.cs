using System.Collections;
using System.Collections.Generic;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FileIO = System.IO.File;

public class LibraryScreen : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button backButton;
    [SerializeField] private InputField searchTitle;

    [SerializeField] private GameComponent component;
    [SerializeField] private LoadingDots loading;
    [SerializeField] private Transform confirmDialog;
    [SerializeField] private Button onYesButton;
    [SerializeField] private Button onNoButton;
    

    private string url;

    private int gameId;
    private GameComponent comp;

    void Start()
    {
        newGame.onClick.AddListener(OnClickNewGame);
        backButton.onClick.AddListener(OnClickBack);
        onYesButton.onClick.AddListener(ConfirmDeleteGame);
        onNoButton.onClick.AddListener(DontDeleteGame);
        SceneDataCarrier.GetData(Constants.GAME_TYPE_KEY, out url);

        if (!url.IsNullEmptyOrWhitespace())
        {
            APICommunication.Instance.StartDownloadFiles(url);
        }
        APICommunication.Instance.StartHealthChecker(url);
    }

    public void InstantiateGamesList(ImageSeqList list)
    {
        foreach (var game in list.data)
        {
            if (!game.deleted)
            {
                GameComponent comp = Instantiate(component, this.transform);
                comp.Init(game,OnDeleteGame);
            }
        }
        loading.gameObject.SetActive(false);
    }

    private void OnDeleteGame(int id, GameComponent comp)
    {
        confirmDialog.gameObject.SetActive(true);
        gameId = id;
        this.comp = comp;
    }

    public void ConfirmDeleteGame()
    {
        confirmDialog.gameObject.SetActive(false);
        APICommunication.Instance.StartDeleteData(gameId, comp);
        gameId = -1;
        comp = null;
    }
    
    public void DontDeleteGame()
    {
        confirmDialog.gameObject.SetActive(false);
        gameId = -1;
        this.comp =null;
    }
    
    private void OnClickNewGame()
    {
        SceneManager.LoadScene("Form");
        /*ImageSeqJsonGet teste = new ImageSeqJsonGet()
        {
            gameTitle = "oi"
        };
        string json = JsonConvert.SerializeObject(teste);
        FileIO.WriteAllText(PATH, json);*/
    }
    
    private void OnClickBack()
    {
        SceneManager.LoadScene("Dashboard");
    }

}
