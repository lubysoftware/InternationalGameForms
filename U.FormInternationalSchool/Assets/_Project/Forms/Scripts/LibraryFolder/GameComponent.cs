using LubyLib.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameComponent : MonoBehaviour
{
    [SerializeField] private Button editButton;
    [SerializeField] private Button _pathButton;
    [SerializeField] private Button deleteButton;

    [SerializeField] private TextMeshProUGUI gameTitle;

    [SerializeField] private LibraryScreen library;

    [SerializeField] private Image fieldBackground;
    [SerializeField] private Image draftIcon;


    private string title;
    private int id;
    private string scene;
    private string _gameType;

    private bool _canClick;

    public void Init(string titleGame, int idGame, string sceneGame, string gameType, Color button, Color field, Color draftColor,bool isDraft)
    {
        title = titleGame;
        id = idGame;
        scene = sceneGame;
        gameTitle.text = title;
        _gameType = gameType;
        
        SetColors(button,field, draftColor,isDraft);
        
        editButton.onClick.AddListener(OnEditButton);
        deleteButton.onClick.AddListener(OnDeleteButton);
        _pathButton.onClick.AddListener(OnPathButtonClick);
        
        _pathButton.gameObject.SetActive(GlobalSettings.Instance.OpenedFromPath);
        fieldBackground.rectTransform.sizeDelta =
            new Vector2(GlobalSettings.Instance.OpenedFromPath ? 1280f : 1365f, 52);

        _canClick = true;
    }

    private void SetColors(Color buttonColor, Color fieldColor,Color draftColor, bool isDraft)
    {
        editButton.image.color = buttonColor;
        deleteButton.image.color = buttonColor;
        _pathButton.image.color = buttonColor;
        //Color draft = new Color(draftColor.r,draftColor.g,draftColor.b,0.7f);
        fieldBackground.color = isDraft? draftColor : fieldColor;
        draftIcon.color = fieldColor;
        draftIcon.gameObject.SetActive(isDraft);
    }
    
    private void OnEditButton()
    {
        if(!_canClick) return;

        _canClick = false;
        
        SceneDataCarrier.AddData(Constants.IS_EDIT, true);
        SceneDataCarrier.AddData(Constants.GAME_EDIT, id);
        SceneManager.LoadScene(scene);
    }

    private void OnDeleteButton()
    {
        if(!_canClick) return;
        
        library.OnDeleteGame(id,this,title );
    }
    
    private void OnPathButtonClick()
    {
        if(!GlobalSettings.Instance.OpenedFromPath || !_canClick) return;
        
        PortalBridge.Instance.AddGameToPathEvent(id, _gameType);
    }
    
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
