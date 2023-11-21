using System;
using LubyLib.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialSceneController : MonoBehaviour
{
    [SerializeField] private GameTypeSO _gameToLoad;
    
    public void LoadQuizList()
    {
        SceneDataCarrier.AddData(Constants.GAME_SO, _gameToLoad);
        SceneManager.LoadScene("Library");
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         LoadQuizList();
    //     }
    // }
}
