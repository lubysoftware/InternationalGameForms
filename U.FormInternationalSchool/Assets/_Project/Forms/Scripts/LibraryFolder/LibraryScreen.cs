using System;
using API;
using International.Api;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FileIO = System.IO.File;

public class LibraryScreen : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField searchTitle;

    [SerializeField] private GameComponent[] component;
    [SerializeField] private LoadingDots loading;
    [SerializeField] private Transform confirmDialog;
    [SerializeField] private Button onYesButton;
    [SerializeField] private Button onNoButton;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button previousPage;

    private string url;

    private int gameId;
    private GameComponent comp;

    private int page = 1;

    private string gameTitle;
    
    private I

    private void Awake()
    {
        newGame.onClick.AddListener(OnClickNewGame);
        backButton.onClick.AddListener(OnClickBack);
        onYesButton.onClick.AddListener(ConfirmDeleteGame);
        onNoButton.onClick.AddListener(DontDeleteGame);
        nextPage.onClick.AddListener(OnClickNext);
        previousPage.onClick.AddListener(OnClickPrevious);
    }

    private void Start()
    {
        SceneDataCarrier.GetData(Constants.GAME_TYPE_KEY, out url);

        //Primeiro checa o status do server
        ApiController.Instance.CheckHealth(OnApiHealthChecked);
    }

    private void OnApiHealthChecked(bool healthStatus)
    {
        //Se o server estiver on, baixa a lista de atividades
        if (healthStatus)
        {
            DownloadGameList();
        }
    }

    private void DownloadGameList()
    {
        APIFactory.GetApi<ImageSequenceApi>().
        
        DownloadData(1);
    }

    public void InstantiateGamesList(ImageSeqList list)
    {
        page = list.meta.page;
        nextPage.interactable = list.meta.countItems > page * list.meta.perPage;
        previousPage.interactable = page > 1;
        for (int i = 0; i < component.Length; i++)
        {
            if (i < list.data.Count)
            {
                if (!list.data[i].deleted)
                {
                    component[i].Init(list.data[i]);
                    component[i].gameObject.SetActive(true);
                }
            }
            else
            {
                component[i].gameObject.SetActive(false);
            }
        }

        loading.gameObject.SetActive(false);
    }

    public void OnDeleteGame(int id, GameComponent comp, string title)
    {
        confirmDialog.gameObject.SetActive(true);
        gameId = id;
        this.comp = comp;
        gameTitle = title;
    }

    public void ConfirmDeleteGame()
    {
        confirmDialog.gameObject.SetActive(false);
        APICommunication.Instance.StartDeleteData(gameId, this, gameTitle);
        gameId = -1;
        comp = null;
        gameTitle = "";
    }

    public void DontDeleteGame()
    {
        confirmDialog.gameObject.SetActive(false);
        gameId = -1;
        this.comp = null;
    }

    private void OnClickNewGame()
    {
        SceneDataCarrier.AddData(Constants.IS_EDIT, false);
        SceneManager.LoadScene("Form");
    }

    private void OnClickBack()
    {
        SceneManager.LoadScene("Dashboard");
    }

    public void OnSearchEndEdit()
    {
        DownloadData(1);
    }

    private void DownloadData(int page)
    {
        loading.gameObject.SetActive(true);
        if (!url.IsNullEmptyOrWhitespace())
        {
            APICommunication.Instance.StartDownloadFiles(url, page, component.Length, searchTitle.text);
        }
    }

    private void OnClickNext()
    {
        DownloadData(page + 1);
    }

    private void OnClickPrevious()
    {
        DownloadData(page - 1);
    }

    public void OnDeletedGame()
    {
        DownloadData(page);
    }
}