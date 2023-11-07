using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using LubyLib.Core.Singletons;
using TMPro;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using File = FrostweepGames.Plugins.WebGLFileBrowser.File;
using FileInfo = System.IO.FileInfo;
using FileIO = System.IO.File;
using Toggle = UnityEngine.UI.Toggle;

public class FormScreen : MonoBehaviour
{
    [DllImport("__Internal")]
    protected static extern void OnGameCreated(int gameId, string gameType);
    
    [SerializeField] protected InputElement title;
    [SerializeField] private InputElement statement_PT, statement_EN;
    [SerializeField] private UploadFileElement audioStatement_PT, audioStatement_EN;
    [SerializeField] private UploadFileElement backgroundImage;
    [SerializeField] protected UploadFileElement backgroundMusic;
    [SerializeField] private Toggle timer;
    [SerializeField] protected InputElement timeMin, timeSec;
    [SerializeField] private Button openMaterialSupportPanel;
    [FormerlySerializedAs("supprotMaterialPanel")] [SerializeField] private SupportMaterialCreation supportMaterialPanel;
    [SerializeField] private InputElement timerBonus;
    [SerializeField] private UploadFileElement titleImage;
    [SerializeField] private Button sendForm;
    [SerializeField] private Transform errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private LoadingDots loading;
    [SerializeField] private Button backButton;
    [SerializeField] private Button previewForm;
    [SerializeField] private Sprite inputImage;

    protected FormBase game = new FormBase();
    
    private Dictionary<string, string> newUrlFiles = new Dictionary<string, string>();
    private Dictionary<string, string> filledUrlFiles = new Dictionary<string, string>();
    private Dictionary<string, string> urlDict = new Dictionary<string, string>();
    private List<string> fields = new() { Constants.IMAGE_TITLE, Constants.MUSIC_BACK, Constants.IMAGE_BACK ,Constants.AUDIO_PT,Constants.AUDIO_EN };

    protected int timeInSec = 0;
    private int bonusTimer = 0;
    private int supportMaterialImgsQtt = 0;

    private List<Material> materials = new List<Material>();

    public int loadFileQtt = 0;
    protected int currentLoad = 0;
    protected bool isLoading;

    protected bool canClick;
    public bool isEdit;
    protected int id;
    [SerializeField]
    protected GameTypeSO so;

    protected List<string> emptyField;
    protected bool hasValidationError;
    protected bool isPreview;
    
    protected virtual void Start()
    {
        sendForm.onClick.AddListener(SendFormData);
        sendForm.onClick.AddListener(ShowPreview);
        backButton.onClick.AddListener(BackButton);
        SetCanClick(true);
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (isEdit)
        {
            SceneDataCarrier.GetData(Constants.GAME_EDIT, out id);
            SendFilesToAPI.Instance.StartDownloadGame(this, so.url, id);
        }
    }

    public virtual void FinishDownloadingGame(string text)
    {
         
    }

    public void BackButton()
    {
        SceneManager.LoadScene("Library");
        SetCanClick(true);
    }

    public void SaveDataFail()
    {
        StopLoading();
        SetCanClick(true);
    }

    public void SetCanClick(bool status)
    {
        canClick = status;
        sendForm.interactable = status;
    }

    protected void StopLoading()
    {
        loading.gameObject.SetActive(false);
    }

    public void SendFormData()
    {
        if (!canClick) return;
        loading.gameObject.SetActive(true);
        SetCanClick(false);
        CheckEmptyBaseFormFields();
    }
    
    public void ShowPreview()
    {
        isPreview = true;
        if (!canClick) return;
        loading.gameObject.SetActive(true);
        SetCanClick(false);
        CheckEmptyBaseFormFields();
    }

    #region BASE_FORM

    private void CheckEmptyBaseFormFields()
    {
        if(emptyField == null)
        {
            emptyField = new List<string>();
        }
        else
        {
            emptyField.Clear();
        }
        
        if (title.InputField.text.IsNullEmptyOrWhitespace())
        {
            title.ActivateErrorMode();
            emptyField.Add("Identificador do jogo");
        }
        else
        {
           DeactivateErrorInput(title);
        }
        
        if (statement_EN.InputField.text.IsNullEmptyOrWhitespace())
        {
            statement_EN.ActivateErrorMode();
            emptyField.Add("Enunciado em Inglês");
        } else
        {
            DeactivateErrorInput(statement_EN);
        }

        if (statement_PT.InputField.text.IsNullEmptyOrWhitespace() )
        {
            statement_PT.ActivateErrorMode();
            emptyField.Add("Enunciado em Português");
        }else
        {
            DeactivateErrorInput(statement_PT);
        }
        
        if (titleImage.UploadedFile == null && titleImage.IsFilled == false)
        {
            titleImage.ActivateErrorMode();
            emptyField.Add("Imagem do enunciado");
        }

        if (backgroundMusic.UploadedFile == null && backgroundMusic.IsFilled == false)
        {
            emptyField.Add("Música de fundo");
            backgroundMusic.ActivateErrorMode();
        }
        
        if (audioStatement_EN.UploadedFile == null && audioStatement_PT.IsFilled == false)
        {
            emptyField.Add("Áudio do enunciado em inglês");
            audioStatement_EN.ActivateErrorMode();
        }
        
        if (audioStatement_PT.UploadedFile == null && audioStatement_PT.IsFilled == false)
        {
            emptyField.Add("Áudio do enunciado em português");
            audioStatement_PT.ActivateErrorMode();
        }
        
        if (backgroundImage.UploadedFile == null && backgroundImage.IsFilled == false)
        {
            emptyField.Add("Imagem de fundo");
            backgroundImage.ActivateErrorMode();
        }
        

        if (timer.isOn)
        {
            if (timeMin.InputField.text.IsNullEmptyOrWhitespace())
            {
                timeMin.ActivateErrorMode();
                emptyField.Add("minutos do timer");
            }else
            {
                timeMin.DeactivateErrorMode(inputImage);
            }
            
            if (timerBonus.InputField.text.IsNullEmptyOrWhitespace())
            {
                timerBonus.ActivateErrorMode();
                emptyField.Add("Bônus do timer");
            }else
            {
                timerBonus.DeactivateErrorMode(inputImage);
            }
        }
        else
        {
            timeInSec = 0;
            bonusTimer = 0;
        }

        CheckEmptyGameFields();

    }
    
    protected virtual void ValidateFields()
    {
        hasValidationError = false;
        if (!CheckBetweenValues(timerBonus,0,100, "Bônus do timer"))
        {
            int.TryParse(timerBonus.InputField.text, out bonusTimer);
            hasValidationError = true;
            return;
        }
        if (timer.isOn)
        {
            timeInSec = CalculateTimeInSec("do jogo", timeMin, timeSec, false);
            if (timeInSec == -1)
            {
                hasValidationError = true;
                timeInSec = 0;
            }
        }
    }
    
    public bool CheckGreatherThanZero(InputElement input, string field)
    {
        int value = 0;
        int.TryParse(input.InputField.text, out value);
        if (value <= 0)
        {
            ShowError(field, ErrorType.GREATER_THAN, new int[]{0});
            input.ActivateErrorMode(true);
            return false;
        }
        input.DeactivateErrorMode(inputImage);
        
        return true;
    }

    public void CheckMinutesInput(InputElement el)
    {
        CheckGreatherThanZero(el, "minutos");
    }
    
    public void CheckSecondsInput(InputElement el)
    {
        CheckBetweenValues(el, 0, 59, "segundos");
    }

    protected int CalculateTimeInSec(string name, InputElement minText, InputElement secText, bool allowZero )
    {
        int min, sec = 0;
        int timeTotal = 0;
        int.TryParse(minText.InputField.text, out min);
        int.TryParse(secText.InputField.text, out sec);
        if (!allowZero)
        {
            if (min + sec <= 0)
            {
                ShowError("Tempo do timer " + name, ErrorType.GREATER_THAN, new int[]{0});
                minText.ActivateErrorMode(true);
                secText.ActivateErrorMode(true);
                return -1;
            }
        }
        else
        {
            if (min + sec < 0)
            {
                ShowError("Tempo do timer " + name, ErrorType.GREATER_OR_EQUAL, new int[]{0});
                minText.ActivateErrorMode(true);
                secText.ActivateErrorMode(true);
                return -1;
            }
        }
        
        if (!CheckGreatherThanZero(minText, "minutos"))
        {
            return -1;
        }

        if (!CheckBetweenValues(secText,0,59, "segundos"))
        {
            return -1;
        }

        timeTotal = min * 60 + sec;
        return timeTotal;
    }

    public void CallCheckBonus()
    {
        CheckBetweenValues(timerBonus,0,100, "Bônus do timer");
    }
    
    protected bool CheckBetweenValues(InputElement input, int min, int max, string field)
    {
        int value  = 0;
        int.TryParse(input.InputField.text, out value);
        if (value < min || value > max)
        {
            ShowError(field, ErrorType.BETWEEN, new int[]{min,max});
           input.ActivateErrorMode(true);
           return false;
        }
        input.DeactivateErrorMode(inputImage);
        return true;
    }

    protected void FillBaseData(BaseGameJson baseForm)
    {
        title.InputField.text = baseForm.gameTitle;
        statement_EN.InputField.text = baseForm.questionStatementEnglishVersion;
        statement_PT.InputField.text = baseForm.questionStatementPortugueseVersion;
        timer.SetIsOnWithoutNotify(baseForm.hasTimer);
        if (baseForm.hasTimer)
        {
            timer.onValueChanged.Invoke(baseForm.hasTimer);
            int min = baseForm.timer / 60;
            int sec = baseForm.timer - min * 60;
            timeMin.InputField.text =  String.Format("{0:00}", min);
            timeSec.InputField.text = String.Format("{0:00}", sec);

            timerBonus.InputField.text = baseForm.bonustimer.ToString();
        }

        if (baseForm.hasSupportMaterial)
        {
            supportMaterialPanel.FillSupportMaterial(baseForm.SupportMaterial);
        }
        
        FillUploadFiles(titleImage,"gameTitleImg",baseForm.gameTitleImageUrl);
        FillUploadFiles(backgroundImage,"background",baseForm.backgroundUrl);
        FillUploadFiles( backgroundMusic,"music_theme",baseForm.backgroundMusicUrl);
        FillUploadFiles( audioStatement_EN,"statement_en",baseForm.questionStatementEnglishAudioUrl);
        FillUploadFiles( audioStatement_PT,"statement_pt",baseForm.questionStatementPortugueseAudioUrl);
        loadFileQtt = loadFileQtt + 5;
    }

    public void FillUploadFiles(UploadFileElement element, string name, string value)
    {
        element.FillData(name,value);
        element.OnFill += OnLoadFile;
        currentLoad++;
        CheckIfMaxQtt();
    }

    protected void CheckIfMaxQtt()
    {
        if (currentLoad == loadFileQtt)
        {
            isLoading = true;
        }
    }

    protected void Update()
    {
        if (isLoading)
        {
            if (loadFileQtt == 0)
            {
                StopLoading();
                isLoading = false;
            }
        }
    }

    public void OnLoadFile(UploadFileElement el)
    {
        loadFileQtt--;
        currentLoad--;
        el.OnFill -= OnLoadFile;
    }
    
    protected void SendBaseFormFiles()
    {
        Debug.Log("send base");
        supportMaterialImgsQtt = 0;
        List<File> files = new List<File>();
        newUrlFiles.Clear();
        filledUrlFiles.Clear();
        urlDict.Clear();
        if (titleImage.UploadedFile != null)
        {
            newUrlFiles.TryAdd(fields[0],titleImage.UploadedFile.fileInfo.name);
            files.Add(titleImage.UploadedFile);
        }
        else
        {
            if (titleImage.IsFilled)
            {
                filledUrlFiles.TryAdd(fields[0],titleImage.url);
            }
            else
            {
                ShowError("Imagem de título do jogo", ErrorType.EMPTY, null);
                return;
            }
        }

        if (backgroundMusic.UploadedFile != null)
        {
            newUrlFiles.TryAdd(fields[1], backgroundMusic.UploadedFile.fileInfo.name);
            files.Add(backgroundMusic.UploadedFile);
        }
        else
        {
            if (backgroundMusic.IsFilled)
            {
                filledUrlFiles.TryAdd(fields[1],backgroundMusic.url);
            }
            else
            {
                ShowError("Música de fundo", ErrorType.EMPTY, null);
                return;
            }
        }

        if (backgroundImage.UploadedFile != null)
        {
            newUrlFiles.TryAdd(fields[2], backgroundImage.UploadedFile.fileInfo.name);
            files.Add(backgroundImage.UploadedFile);
        }
        else
        {
            if (backgroundImage.IsFilled)
            {
                filledUrlFiles.TryAdd(fields[2],backgroundImage.url);
            }
            else
            {
                ShowError("Imagem de fundo", ErrorType.EMPTY, null);
                return;
            }
        }

        if (audioStatement_PT.UploadedFile != null)
        {
            newUrlFiles.TryAdd(fields[3], audioStatement_PT.UploadedFile.fileInfo.name);
            files.Add(audioStatement_PT.UploadedFile);
        }
        else
        {
            if (audioStatement_PT.IsFilled)
            {
                filledUrlFiles.TryAdd(fields[3],audioStatement_PT.url);
            }
            else
            {
                ShowError("Áudio do enunciado em Português", ErrorType.EMPTY, null);
                return;
            }
        }

        if (audioStatement_EN.UploadedFile != null)
        {
            newUrlFiles.TryAdd(fields[4], audioStatement_EN.UploadedFile.fileInfo.name);
            files.Add(audioStatement_EN.UploadedFile);
        }
        else
        {
            if (audioStatement_EN.IsFilled)
            {
                filledUrlFiles.TryAdd(fields[4],audioStatement_EN.url);
            }
            else
            {
                ShowError("Áudio do enunciado em inglês", ErrorType.EMPTY, null);
                return;
            }
        }

        materials = supportMaterialPanel.GetSupportMaterial();

        if (materials != null && materials.Count > 0)
        {
            foreach (Material mat in materials)
            {
                if (!mat.isText)
                {
                    if (mat.text == null)
                    {
                        if (mat.file != null)
                        {
                            files.Add(mat.file);
                            supportMaterialImgsQtt++;
                        }
                    }
                }
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
                Debug.Log("upload base");
                SendFilesToAPI.Instance.StartUploadFiles(this,files, true);
            }
            else
            {
                SerializeBaseFormData(null);
            }
            
        }

    }

    public virtual void SerializeBaseFormData(string[] urls)
    {
        Debug.Log("serialize base");
        List<SupportMaterial> supportMaterial = new List<SupportMaterial>();
        if (urls != null)
        {
            if (urls.Length == newUrlFiles.Count + supportMaterialImgsQtt)
            {
                int urlCount = 0;
                for (int i = 0; i< fields.Count; i++)
                {
                    if (!filledUrlFiles.ContainsKey(fields[i]))
                    {
                        newUrlFiles[fields[i]] = urls[urlCount];
                        urlCount++;
                    }
                }
                
                urlDict = newUrlFiles;
                urlDict.AddRange(filledUrlFiles);
                /*foreach (var str in urlDict)
                {
                    Debug.LogError(str.Key + " " + str.Value);
                }*/
                
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
                        if (materials[i].text != null)
                        {
                            supportMaterial.Add(new SupportMaterial()
                            {
                                position = i,
                                material = materials[i].text,
                                materialType = "IMAGE"
                            });
                        }
                        else
                        {
                            supportMaterial.Add(new SupportMaterial()
                            {
                                position = i,
                                material = urls[urlCount],
                                materialType = "IMAGE"
                            });
                            urlCount++;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("error uploading images");
            }
        }
        else
        {
            urlDict = filledUrlFiles;
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
                    if (materials[i].text != null)
                    {
                        supportMaterial.Add(new SupportMaterial()
                        {
                            position = i,
                            material = materials[i].text,
                            materialType = "IMAGE"
                        });
                    }
                }
            }
        }

        game = new FormBase()
        {
            gameTitle = title.InputField.text,
            backgroundMusicUrl = urlDict[Constants.MUSIC_BACK],
            backgroundUrl = urlDict[Constants.IMAGE_BACK],
            bonustimer = bonusTimer,
            gameTitleImageUrl = urlDict[Constants.IMAGE_TITLE],
            hasSupportMaterial =  supportMaterial.Count > 0,
            supportMaterial = supportMaterial.Count > 0? supportMaterial : null,
            hasTimer = timer.isOn,
            questionStatementEnglishAudioUrl = urlDict[Constants.AUDIO_EN],
            questionStatementEnglishVersion = statement_EN.InputField.text,
            questionStatementPortugueseAudioUrl = urlDict[Constants.AUDIO_PT],
            timer = timeInSec,
            questionStatementPortugueseVersion = statement_PT.InputField.text
        };
        SendGameFiles();

    }

    #endregion


    #region GAME_FORM

    protected virtual void CheckEmptyGameFields()
    {
        
    }
    
    
    protected virtual void SendGameFiles()
    {
        
    }

    public virtual void SerializeGameData(string[] urls)
    {
        
    }
    
    public virtual void SendGameInfoToPortal(string responseJson)
    {
        Debug.Log("[DEBUG] Informação do game enviada para o portal.");
// #if UNITY_WEBGL && !UNITY_EDITOR
//         try
//         {
//             GameCreationResponse response = JsonConvert.DeserializeObject<GameCreationResponse>(responseJson);
//             OnGameCreated(response.id, response.gameType);
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError(ex.Message);
//         }
// #endif
    }
    
    public virtual void ShowPreviewPortal(string responseJson)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            ShowPreview(response.id, response);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
#endif
    }

    #endregion
    
    public void ShowError(string field, ErrorType type, int[] values)
    {
        string error = "error";
        switch (type)
        {
            case ErrorType.ALL_FIELDS:
                error = "Todos os campos obrigatórios devem ser preenchidos.";
                break;
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
        SaveDataFail();
    }
    

    public void DeactivateErrorInput(InputElement el)
    {
        el.DeactivateErrorMode(inputImage);
    }


}

public enum ErrorType
{
    EMPTY,
    GREATER_THAN,
    LESS_THAN,
    GREATER_OR_EQUAL,
    BETWEEN, 
    CUSTOM,
    ALL_FIELDS
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
