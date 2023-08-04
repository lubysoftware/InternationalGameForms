using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core.Singletons;
using TMPro;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using File = FrostweepGames.Plugins.WebGLFileBrowser.File;
using FileIO = System.IO.File;

public class FormScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private TMP_InputField statement_PT, statement_EN;
    [SerializeField] private UploadFileElement audioStatement_PT, audioStatement_EN;
    [SerializeField] private UploadFileElement backgroundImage;
    [SerializeField] private UploadFileElement backgroundMusic;
    [SerializeField] private Toggle timer;
    [SerializeField] private TMP_InputField timeMin, timeSec;
    [SerializeField] private Button openMaterialSupportPanel;
    [SerializeField] private TMP_InputField timerBonus;
    [SerializeField] private UploadFileElement titleImage;
    [SerializeField] private TMP_InputField failsPenalty;
    [SerializeField] private Button sendForm;
    [SerializeField] private ImageSequencingPanel panel;
    
    private readonly string PATH = Application.dataPath + "/Teste.json";

    private bool isbaseForm;

    private FormBase game = new FormBase();
    
    private Dictionary<string, string> urlFiles = new Dictionary<string, string>();
    private List<string> fields = new() { Const.IMAGE_TITLE, Const.MUSIC_BACK, Const.IMAGE_BACK ,Const.AUDIO_PT,Const.AUDIO_EN };
    void Start()
    {
        SendFilesToAPI.Instance.OnUploadFiles += SerializeFormData;
        Debug.LogError(gameObject.name);
        Debug.LogError(PATH);
    }

    public void SendFormData()
    {
        
    }

    private void SetIsBaseForm(bool value)
    {
        isbaseForm = value;
        Debug.LogError(isbaseForm);
    }
    
    private void SerializeFormData(string[] urls)
    {
        if (isbaseForm)
        {
            for (int i = 0; i< fields.Count; i++)
            {
                urlFiles[fields[i]] = urls[i];
            }

            game = new FormBase()
            {
                gameTitle = title.text,
                backgroundMusicUrl = urlFiles[Const.MUSIC_BACK],
                backgroundUrl = urlFiles[Const.IMAGE_BACK],
                bonusTimer = timerBonus.text,
                gameTitleImageUrl = urlFiles[Const.IMAGE_TITLE],
                //tratar
                hasSupportMaterial = true,
                hasTimer = timer.isOn,
                questionStatementEnglishAudioUrl = urlFiles[Const.AUDIO_EN],
                questionStatementEnglishVersion = statement_EN.text,
                questionStatementPortugueseAudioUrl = urlFiles[Const.AUDIO_PT],
                //tratar
                timer = timeMin.text,
                questionStatementPortugueseVersion = statement_PT.text
            };
            SetIsBaseForm(false);
            SendFilesToAPI.Instance.StartUploadFiles(panel.GetImages());
        }
        else
        {
            List<Sequence> listSeq = new List<Sequence>();
            for (int i = 0; i < urls.Length; i++)
            {
                listSeq.Add(new Sequence(){ position = i, imageUrl = urls[i]});
            }
            
            FormImageSequence completeForm = new FormImageSequence()
            {
                game = this.game,
                gameData =  new ImageSequence()
                {
                    failsPenalty = failsPenalty.text,
                    sequences = listSeq
                }
            };
            
            string json = JsonConvert.SerializeObject(completeForm);

            if (FileIO.Exists(PATH))
            {
                FileIO.Delete(PATH);
            }
           
            FileIO.WriteAllText(PATH, json);
        }
    }

    public void SendFiles()
    {
        SetIsBaseForm(true);
        List<File> files = new List<File>();
        
        urlFiles.Add(fields[0],titleImage.UploadedFile.fileInfo.name);
        files.Add(titleImage.UploadedFile);
        
        urlFiles.Add(fields[1],backgroundMusic.UploadedFile.fileInfo.name);
        files.Add(backgroundMusic.UploadedFile);
        
        urlFiles.Add(fields[2],backgroundImage.UploadedFile.fileInfo.name);
        files.Add(backgroundImage.UploadedFile);
        
        urlFiles.Add(fields[3],audioStatement_PT.UploadedFile.fileInfo.name);
        files.Add(audioStatement_PT.UploadedFile);
        
        urlFiles.Add(fields[4],audioStatement_PT.UploadedFile.fileInfo.name);
        files.Add(audioStatement_EN.UploadedFile);

        SendFilesToAPI.Instance.StartUploadFiles(files);
       
    }


}

[Serializable]
public class FormBase
{
    public string gameTitle;
    public string gameTitleImageUrl;
    public string backgroundUrl;
    public string backgroundMusicUrl;
    public string questionStatementPortugueseVersion;
    public string questionStatementEnglishVersion;
    public string questionStatementPortugueseAudioUrl;
    public string questionStatementEnglishAudioUrl;
    public bool hasSupportMaterial;
    public bool hasTimer;
    public string timer;
    public string bonusTimer;
}

[Serializable]
public struct Sequence
{
    public int position;
    public string imageUrl;
}

[Serializable]
public class ImageSequence
{
    public string failsPenalty;
    public List<Sequence> sequences;
}

[Serializable]
public class FormImageSequence
{
    public FormBase game;
    public ImageSequence gameData;
}

public class Const 
{
    public const string IMAGE_TITLE = "gameTitleImageUrl";
    public const string IMAGE_BACK = "backgroundUrl";
    public const string MUSIC_BACK = "backgroundMusicUrl";
    public const string AUDIO_PT = "questionStatementPortugueseAudioUrl";
    public const string AUDIO_EN = "questionStatementEnglishAudioUrl";
}
