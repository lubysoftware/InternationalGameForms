using LubyLib.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameComponent : MonoBehaviour
{
    [SerializeField] private Button editButton;

    [SerializeField] private Button deleteButton;

    [SerializeField] private TextMeshProUGUI gameTitle;

    [SerializeField] private LibraryScreen library;

    [SerializeField] private Image fieldBackground;
    

    private string title;
    private int id;
    private string scene;

    public void Init(string titleGame, int idGame, string sceneGame, Color button, Color field)
    {
        title = titleGame;
        id = idGame;
        scene = sceneGame;
        gameTitle.text = title;
        SetColors(button,field);
    }

    private void SetColors(Color button, Color field)
    {
        editButton.image.color = button;
        deleteButton.image.color = button;
        fieldBackground.color = field;
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
