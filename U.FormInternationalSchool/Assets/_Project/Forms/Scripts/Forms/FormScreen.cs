using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using LubyLib.Core.Singletons;
using TMPro;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using File = FrostweepGames.Plugins.WebGLFileBrowser.File;
using FileIO = System.IO.File;
using Toggle = UnityEngine.UI.Toggle;

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
    [FormerlySerializedAs("supprotMaterialPanel")] [SerializeField] private SupportMaterialCreation supportMaterialPanel;
    [SerializeField] private TMP_InputField timerBonus;
    [SerializeField] private UploadFileElement titleImage;
    [SerializeField] private Button sendForm;
    [SerializeField] private Transform errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;

    private bool IsbaseForm { get; set; }

    protected FormBase game = new FormBase();
    
    private Dictionary<string, string> newUrlFiles = new Dictionary<string, string>();
    private Dictionary<string, string> filledUrlFiles = new Dictionary<string, string>();
    private Dictionary<string, string> urlDict = new Dictionary<string, string>();
    private List<string> fields = new() { Constants.IMAGE_TITLE, Constants.MUSIC_BACK, Constants.IMAGE_BACK ,Constants.AUDIO_PT,Constants.AUDIO_EN };

    private int timeInSec = 0;
    private int bonusTimer = 0;
    private int supportMaterialImgsQtt = 0;

    private List<Material> materials = new List<Material>();

    protected virtual void Start()
    {
        sendForm.onClick.AddListener(SendFormData);
        SendFilesToAPI.Instance.OnUploadFiles += SerializeBaseFormData;
    }

    public virtual void FinishDownloadingGame(string text)
    {
        
    }

    public void SendFormData()
    {
        CheckBaseFormFields();
    }

    #region BASE_FORM

    private void CheckBaseFormFields()
    {
        if (title.text.IsNullEmptyOrWhitespace())
        {
            ShowError("Identificador do jogo", ErrorType.EMPTY, null);
            return;
        }

        if (titleImage.UploadedFile == null && titleImage.IsFilled == false)
        {
            ShowError("Imagem de título do jogo", ErrorType.EMPTY, null);
            return;
        }

        if (backgroundMusic.UploadedFile == null && backgroundMusic.IsFilled == false)
        {
            ShowError("Música de fundo", ErrorType.EMPTY, null);
            return;
        }

        if (statement_EN.text.IsNullEmptyOrWhitespace())
        {
            ShowError("Enunciado em Inglês", ErrorType.EMPTY, null);
            return;
        }
        
        if (audioStatement_EN.UploadedFile == null && audioStatement_PT.IsFilled == false)
        {
            ShowError("Áudio do enunciado em Inglês", ErrorType.EMPTY, null);
            return;
        }
        
        if (statement_PT.text.IsNullEmptyOrWhitespace() )
        {
            ShowError("Enunciado em Português", ErrorType.EMPTY, null);
            return;
        }
        
        if (audioStatement_PT.UploadedFile == null && audioStatement_PT.IsFilled == false)
        {
            ShowError("Áudio do enunciado em Português", ErrorType.EMPTY, null);
            return;
        }
        
        if (backgroundImage.UploadedFile == null && backgroundImage.IsFilled == false)
        {
            ShowError("Imagem de fundo", ErrorType.EMPTY, null);
            return;
        }

        if (timer.isOn)
        {
            if (timeMin.text.IsNullEmptyOrWhitespace())
            {
                ShowError("Minutos do timer", ErrorType.EMPTY, null);
                return;
            }

            int min, sec = -1;
            int.TryParse(timeMin.text, out min);
            int.TryParse(timeSec.text, out sec);
            if (min + sec <= 0)
            {
                ShowError("Tempo do timer", ErrorType.GREATER_THAN, new int[]{0});
                return;
            }
            if (min < 0)
            {
                ShowError("Minutos do timer", ErrorType.GREATER_OR_EQUAL, new int[]{0});
                return;
            }

            if (sec < 0 || sec > 59)
            {
                ShowError("Segundos do timer", ErrorType.BETWEEN, new int[]{0,59});
                return;
            }

            timeInSec = min * 60 + sec;
        }
        else
        {
            
            timeInSec = 0;
        }

        bonusTimer = 0;
        int.TryParse(timerBonus.text, out bonusTimer);
        if (bonusTimer < 0 || bonusTimer > 100)
        {
            ShowError("Bônus do timer", ErrorType.BETWEEN, new int[]{0,100});
        }
        
        CheckGameFields();

    }

    protected void FillGameData(ImageSeqJsonGet baseForm)
    {
        title.text = baseForm.gameTitle;
        statement_EN.text = baseForm.questionStatementEnglishVersion;
        statement_PT.text = baseForm.questionStatementPortugueseVersion;
        if (baseForm.hasTimer)
        {
            timer.onValueChanged.Invoke(baseForm.hasTimer);
            int min = baseForm.timer % 60;
            int sec = baseForm.timer - min * 60;
            timeMin.text = min.ToString();
            timeSec.text = sec.ToString();
            timerBonus.text = baseForm.bonustimer.ToString();
        }

        if (baseForm.hasSupportMaterial)
        {
           // supportMaterialPanel.FillSupportMaterial(baseForm.SupportMaterial);
        }
        
        titleImage.FillData("gameTitleImg",baseForm.gameTitleImageUrl);
        backgroundImage.FillData("background",baseForm.backgroundUrl);
        backgroundMusic.FillData("music_theme",baseForm.backgroundMusicUrl);
        audioStatement_EN.FillData("statement_en",baseForm.questionStatementEnglishAudioUrl);
        audioStatement_PT.FillData("statement_pt",baseForm.questionStatementPortugueseAudioUrl);
    }
    
    protected void SendBaseFormFiles()
    {
        IsbaseForm = true;
        supportMaterialImgsQtt = 0;
        List<File> files = new List<File>();
        newUrlFiles.Clear();
        if (titleImage.UploadedFile != null)
        {
            newUrlFiles.Add(fields[0],titleImage.UploadedFile.fileInfo.name);
            files.Add(titleImage.UploadedFile);
        }
        else
        {
            if (titleImage.IsFilled)
            {
                filledUrlFiles.Add(fields[0],titleImage.url);
            }
            else
            {
                ShowError("Imagem de título do jogo", ErrorType.EMPTY, null);
                return;
            }
        }

        if (backgroundMusic.UploadedFile != null)
        {
            newUrlFiles.Add(fields[1], backgroundMusic.UploadedFile.fileInfo.name);
            files.Add(backgroundMusic.UploadedFile);
        }
        else
        {
            if (backgroundMusic.IsFilled)
            {
                filledUrlFiles.Add(fields[1],backgroundMusic.url);
            }
            else
            {
                ShowError("Música de fundo", ErrorType.EMPTY, null);
                return;
            }
        }

        if (backgroundImage.UploadedFile != null)
        {
            newUrlFiles.Add(fields[2], backgroundImage.UploadedFile.fileInfo.name);
            files.Add(backgroundImage.UploadedFile);
        }
        else
        {
            if (backgroundImage.IsFilled)
            {
                filledUrlFiles.Add(fields[2],backgroundImage.url);
            }
            else
            {
                ShowError("Imagem de fundo", ErrorType.EMPTY, null);
                return;
            }
        }

        if (audioStatement_PT.UploadedFile != null)
        {
            newUrlFiles.Add(fields[3], audioStatement_PT.UploadedFile.fileInfo.name);
            files.Add(audioStatement_PT.UploadedFile);
        }
        else
        {
            if (audioStatement_PT.IsFilled)
            {
                filledUrlFiles.Add(fields[3],audioStatement_PT.url);
            }
            else
            {
                ShowError("Áudio do enunciado em Português", ErrorType.EMPTY, null);
                return;
            }
        }

        if (audioStatement_EN.UploadedFile != null)
        {
            newUrlFiles.Add(fields[4], audioStatement_EN.UploadedFile.fileInfo.name);
            files.Add(audioStatement_EN.UploadedFile);
        }
        else
        {
            if (audioStatement_EN.IsFilled)
            {
                filledUrlFiles.Add(fields[4],audioStatement_EN.url);
            }
            else
            {
                ShowError("Áudio do enunciado em inglês", ErrorType.EMPTY, null);
                return;
            }
        }

        materials = supportMaterialPanel.GetSupportMaterial();
        
        foreach (Material mat in materials)
        {
            if (!mat.isText)
            {
                files.Add(mat.file);
                supportMaterialImgsQtt++;
            }
        }
        
        if (files.Count < newUrlFiles.Count)
        {
            ShowError("Por favor, preencha todos os campos de arquivos.", ErrorType.CUSTOM, null);
        }
        else
        {
            if (files.Count > 0)
            {
                SendFilesToAPI.Instance.StartUploadFiles(files);
            }
            else
            {
                SerializeBaseFormData(null);
            }
            
        }

    }

    private void SerializeBaseFormData(string[] urls)
    {
        if (IsbaseForm)
        {
            List<SupportMaterial> supportMaterial = new List<SupportMaterial>();
            if (urls != null)
            {
                if (urls.Length == newUrlFiles.Count + supportMaterialImgsQtt)
                {
                    for (int i = 0; i< newUrlFiles.Count; i++)
                    {
                        newUrlFiles[fields[i]] = urls[i];
                    }


                    urlDict = newUrlFiles;
                    urlDict.AddRange(filledUrlFiles);
                    
                    int urlsSupportMaterial = fields.Count;
                    for (int i = 0; i< materials.Count; i++)
                    {
                        if (materials[i].isText)
                        {
                            supportMaterial.Add(new SupportMaterial()
                            {
                                position = i,
                                material = materials[i].text,
                                materialType = "TEXT"
                            
                            });
                        }
                        else
                        {
                            supportMaterial.Add(new SupportMaterial()
                            {
                                position = i,
                                material = urls[urlsSupportMaterial],
                                materialType = "IMAGE"
                            });
                            urlsSupportMaterial++;
                        }
                    }
                }
                else
                {
                    Debug.LogError("error uploading images");
                }
            }

            game = new FormBase()
            {
                gameTitle = title.text,
                backgroundMusicUrl = urlDict[Constants.MUSIC_BACK],
                backgroundUrl = urlDict[Constants.IMAGE_BACK],
                bonustimer = bonusTimer,
                gameTitleImageUrl = urlDict[Constants.IMAGE_TITLE],
                hasSupportMaterial =  supportMaterial.Count > 0,
                supportMaterial = supportMaterial.Count > 0? supportMaterial : null,
                hasTimer = timer.isOn,
                questionStatementEnglishAudioUrl = urlDict[Constants.AUDIO_EN],
                questionStatementEnglishVersion = statement_EN.text,
                questionStatementPortugueseAudioUrl = urlDict[Constants.AUDIO_PT],
                timer = timeInSec,
                questionStatementPortugueseVersion = statement_PT.text
            };
            IsbaseForm = false;
            SendGameFiles();
        }
        else
        {
           SerializeGameData(urls);
        }
    }

    #endregion


    #region GAME_FORM

    protected virtual void CheckGameFields()
    {
        
    }
    protected virtual void SendGameFiles()
    {
        
    }

    protected virtual void SerializeGameData(string[] urls)
    {
        
    }
    

    #endregion
    
    
    protected void ShowError(string field, ErrorType type, int[] values)
    {
        string error = "error";
        switch (type)
        {
            case ErrorType.EMPTY:
                error = String.Format("O campo {0} deve ser preenchido.",field);
                break;
            case ErrorType.LESS_THAN:
                error = String.Format("O campo {0} deve ser preenchido com um valor menor que {1}.",field, values[0]);
                break;
            case ErrorType.GREATER_THAN:
                error = String.Format("O campo {0} deve ser preenchido com um valor maior que {1}.",field, values[0]);
                break;
            case ErrorType.GREATER_OR_EQUAL:
                error = String.Format("O campo {0} deve ser preenchido com um valor maior  ou igual a {1}.",field, values[0]);
                break;
            case ErrorType.BETWEEN:
                error = String.Format("O campo {0} deve ser preenchido com um valor maior que {1} e menor que {2}.",field, values[0], values[1]);
                break;
            case ErrorType.CUSTOM:
                error = field;
                break;
        }

        errorText.text = error;
        errorPanel.gameObject.SetActive(true);
    }

}

public enum ErrorType
{
    EMPTY,
    GREATER_THAN,
    LESS_THAN,
    GREATER_OR_EQUAL,
    BETWEEN, 
    CUSTOM
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
    public int timer;
    public int bonustimer;
    public List<SupportMaterial> supportMaterial;
}
