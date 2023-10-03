using LubyLib.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTypeButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button button;
    private Image image;

    private GameTypeSO so;
    
    public void Init(GameTypeSO so)
    {
        description.text = so.description;
        title.text = so.title;
        GetComponent<Image>().sprite = so.sprite;
        button.interactable = so.isActive;
        this.so = so;
        if (so.isActive)
        {
            title.color = new Color(0, 0.2f, 0.3f, 1);
            description.color = new Color(0, 0.2f, 0.3f, 1);
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        SceneDataCarrier.AddData(Constants.GAME_SO, so);
        SceneManager.LoadScene("Library");
    }
}
