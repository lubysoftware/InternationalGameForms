using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FileIO = System.IO.File;

public class LibraryScreen : MonoBehaviour
{
    [SerializeField] private Button newGame;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField searchTitle;
    [SerializeField] private Button searchButton;
    
    [SerializeField] private GameComponent[] component;
    [SerializeField] private LoadingDots loading;
    [SerializeField] private DeleteGamePanel confirmDialog;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button previousPage;
    [SerializeField] private TextMeshProUGUI pagesText;
    [SerializeField] private Image scroll;
    [SerializeField] private Image scrollBack;
    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private GameSettingsPanel settingsPanel;
    [SerializeField] private Button settings;
    
    private GameTypeSO so;

    private int gameId;
    private GameComponent comp;

    private int page = 1;

    private string gameTitle;

    private void Start()
    {
        newGame.onClick.AddListener(OnClickNewGame);
        backButton.onClick.AddListener(OnClickBack);
        nextPage.onClick.AddListener(OnClickNext);
        previousPage.onClick.AddListener(OnClickPrevious);
        settings.onClick.AddListener(OpenSettings);
        searchButton.onClick.AddListener(OnSearchButton);
        SceneDataCarrier.AddData(Constants.GAME_SETTINGS,new DefaultSettings());
        SceneDataCarrier.GetData(Constants.GAME_SO, out so);
        loading.SetColors(so.colors[0], so.colors[1]);
        SetLibColors();
        DownloadData(1);
        APICommunication.Instance.StartHealthChecker(so.url);
        APICommunication.Instance.StartDownloadDefaultSettings(this,so.gameType.ToString());
    }

    public void SetLibColors()
    {
        //round buttons
        backButton.image.color = so.colors[0];
        nextPage.image.color = so.colors[0];
        previousPage.image.color = so.colors[0];

        //header
        newGame.image.color = so.colors[1];
        settings.image.color = so.colors[0];
        searchTitle.image.color = so.colors[3];
        searchButton.image.color = so.colors[0];
        title.text = so.title;

        //scroll
        scroll.color = so.colors[1];
        scrollBack.color = so.colors[3];

        //deletePanel
        confirmDialog.SetPanelColors(so.colors[0], so.colors[3]);

        settingsPanel.SetPanelColors(so.colors[0], so.colors[3], so.colors[2], so.colors[1], so.colors[4], so.gameType);
    }

    private void OpenSettings()
    {
        settingsPanel.gameObject.SetActive(true);
    }

    public void InstantiateGamesList(GameList list)
    {
        page = list.meta.page;
        nextPage.interactable = list.meta.countItems > page * list.meta.perPage;
        pagesText.text = page + "/" + list.meta.lastPage;
        previousPage.interactable = page > 1;
        for (int i = 0; i < component.Length; i++)
        {
            if (i < list.data.Count)
            {
                if (!list.data[i].deleted)
                {
                    component[i].Init(list.data[i].gameTitle, list.data[i].id, so.scene, list.data[i].gameType,
                        so.colors[0], so.colors[2]);
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
        confirmDialog.SetNewDelete(ConfirmDeleteGame, DontDeleteGame, title);
        confirmDialog.gameObject.SetActive(true);
        gameId = id;
        this.comp = comp;
        gameTitle = title;
    }

    public void ConfirmDeleteGame()
    {
        APICommunication.Instance.StartDeleteData(gameId, this, gameTitle);
        gameId = -1;
        comp = null;
        gameTitle = "";
    }

    public void DontDeleteGame()
    {
        gameId = -1;
        this.comp = null;
    }

    private void OnClickNewGame()
    {
        SceneDataCarrier.AddData(Constants.IS_EDIT, false);
        SceneManager.LoadScene(so.scene);
    }

    private void OnClickBack()
    {
        SceneManager.LoadScene("Dashboard");
    }

    public void OnSearchButton()
    {
        DownloadData(1);
    }

    private void DownloadData(int page)
    {
        loading.gameObject.SetActive(true);
        if (!so.url.IsNullEmptyOrWhitespace())
        {
            APICommunication.Instance.StartDownloadFiles(so.url, page, 7, searchTitle.text);
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

    public void SaveSettingsData(string data)
    {
        DefaultSettings settings = JsonConvert.DeserializeObject<DefaultSettings>(data);
        SceneDataCarrier.AddData(Constants.GAME_SETTINGS, settings );
    } 

}