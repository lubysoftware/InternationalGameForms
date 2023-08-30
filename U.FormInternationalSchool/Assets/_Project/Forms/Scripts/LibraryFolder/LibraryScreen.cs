using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
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
    [SerializeField] private DeleteGamePanel confirmDialog;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button previousPage;
    [SerializeField] private TextMeshProUGUI pagesText;
    [SerializeField] private Image scroll;
    [SerializeField] private Image scrollBack;
    [SerializeField] private TextMeshProUGUI title;

    private GameTypeSO so;

    private int gameId;
    private GameComponent comp;

    private int page = 1;

    private string gameTitle;

    void Start()
    {
        newGame.onClick.AddListener(OnClickNewGame);
        backButton.onClick.AddListener(OnClickBack);
        nextPage.onClick.AddListener(OnClickNext);
        previousPage.onClick.AddListener(OnClickPrevious);
        SceneDataCarrier.GetData(Constants.GAME_SO, out so);
        loading.SetColors(so.colors[0], so.colors[1]);
        SetLibColors();
        DownloadData(1);
        APICommunication.Instance.StartHealthChecker(so.url);
    }

    public void SetLibColors()
    {
        //round buttons
        backButton.image.color = so.colors[4];
        nextPage.image.color = so.colors[4];
        previousPage.image.color = so.colors[4];

        //header
        newGame.image.color = so.colors[1];
        searchTitle.image.color = so.colors[3];
        title.text = so.title;
        
        //scroll
        scroll.color = so.colors[1];
        scrollBack.color = so.colors[3];

        //deletePanel
        confirmDialog.SetPanelColors(so.colors[0], so.colors[3]);
    }

    public void InstantiateGamesList(ImageSeqList list)
    {
        page = list.meta.page;
        nextPage.interactable = list.meta.countItems > page * list.meta.perPage;
        pagesText.text = page + "/" + list.meta.lastPage;
        previousPage.interactable = page > 1;
        for (int i=0; i < component.Length; i++)
        {
            if (i < list.data.Count)
            {
                if (!list.data[i].deleted)
                {
                    component[i].Init(list.data[i].gameTitle,list.data[i].id,so.scene, so.colors[4], so.colors[2] );
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
        confirmDialog.SetNewDelete(ConfirmDeleteGame,DontDeleteGame,title);
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
        this.comp =null;
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

    public void OnSearchEndEdit()
    {
        DownloadData(1);
    }

    private void DownloadData(int page)
    {
        loading.gameObject.SetActive(true);
        if (!so.url.IsNullEmptyOrWhitespace())
        {
            APICommunication.Instance.StartDownloadFiles(so.url,page,7, searchTitle.text);
        }
    }

    private void OnClickNext()
    {
        DownloadData(page +1);
    }

    private void OnClickPrevious()
    {
        DownloadData(page -1);
    }

    public void OnDeletedGame()
    {
        DownloadData(page);
    }
}
