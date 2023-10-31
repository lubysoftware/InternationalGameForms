using System;
using System.Collections;
using System.Collections.Generic;
using API;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Proyecto26;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown questionType;
    [SerializeField] private TMP_Dropdown alternativeType;
    [SerializeField] private TMP_Dropdown alternativeQtt;
    [SerializeField] private InputElement statementPT_text;
    [SerializeField] private InputElement statementEN_text;
    [SerializeField] private UploadFileElement statementPT_audio;
    [SerializeField] private UploadFileElement statementEN_audio;
    [SerializeField] private UploadFileElement extra_statementAudio;
    [SerializeField] private UploadFileElement extra_statementImg;
    [SerializeField] private AlternativeGroup alternativesGroup;
    [SerializeField] private Transform alternativeGroupContainer;

    [Header("Settings Buttons")] [SerializeField]
    private Button moveUpperBtn;

    [SerializeField] private Button moveDownBtn;
    [SerializeField] private Button deleteQuestion;
    [SerializeField] private GameObject panelSetting;
    [SerializeField] private Button settingsButton;

    [Header("Question error view")] [SerializeField]
    private Image background;

    [SerializeField] GameObject errorSymbol;
    [SerializeField] private Sprite[] backSprite;

    private int previousQttDropdown;
    private QuestionsGroup.InputType previousTypeDropdown;

    [Space(15)] [Header("Layouts")] [SerializeField]
    private LayoutGroup layout;

    [SerializeField] private LayoutGroup layout2;
    [SerializeField] private LayoutGroup layout3;
    
    private Dictionary<string, string> filledUrls;
    
    private const string EN = "EN_AUDIO";
    private const string PT = "PT_AUDIO";
    private const string FILE = "URL_STATEMENT";
    
    void Start()
    {
        AddListeners();
        alternativeQtt.value = 3;
    }

    #region Subscriptions

    private void AddListeners()
    {
        moveDownBtn.onClick.AddListener(MoveDown);
        moveUpperBtn.onClick.AddListener(MoveUpper);
        deleteQuestion.onClick.AddListener(DeleteButton);
        questionType.onValueChanged.AddListener(OnChangeQuestionType);
        alternativeType.onValueChanged.AddListener(OnChangeAlternativeTypeDrop);
        alternativeQtt.onValueChanged.AddListener(OnChangeAlternativeQttDrop);
        settingsButton.onClick.AddListener(ShowPanel);
        OnChangeAlternativeTypeDrop(0);
    }

    #endregion

    #region FillData

    public void SetQuestionType(int value)
    {
        OnChangeQuestionType(value);
    }

    #endregion

    #region Settings

    private void MoveUpper()
    {
        int index = transform.GetSiblingIndex();
        if (index > 0)
        {
            transform.SetSiblingIndex(index - 1);
        }
        UpdateButtonsInteraction();
        ShowPanel();
        QuestionsGroup.Instance.UpdateCanvas();
    }

    public void ShowPanel()
    {
        UpdateButtonsInteraction();
        panelSetting.gameObject.SetActive(!panelSetting.activeInHierarchy);
    }

    private void UpdateButtonsInteraction()
    {
        int index = transform.GetSiblingIndex();

        moveDownBtn.interactable = index < QuestionsGroup.Instance.QuestionsQtt-1;
        moveUpperBtn.interactable = index > 0;
    }
    
    private void MoveDown()
    {
        int index = transform.GetSiblingIndex();
        if (index < QuestionsGroup.Instance.QuestionsQtt-1)
        {
            transform.SetSiblingIndex(index + 1);
        }
        UpdateButtonsInteraction();
        ShowPanel();
        QuestionsGroup.Instance.UpdateCanvas();
    }

    public void UpdateCanvas()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout2.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout3.transform as RectTransform);
        QuestionsGroup.Instance.UpdateCanvas();
    }

    private void DeleteButton()
    {
        QuestionsGroup.Instance.ShowConfirmPanel("Confirma excluir a questao?",DeleteQuestion,ShowPanel );
    }

    private void DeleteQuestion()
    {
        DestroyImmediate(this.gameObject);
        QuestionsGroup.Instance.CheckAddQuestionButton();
        QuestionsGroup.Instance.UpdateCanvas();
    }


#endregion

    #region Dropdowns

    private void OnChangeQuestionType(int value)
    {
        if (value == (int)QuestionsGroup.InputType.AUDIO)
        {
            extra_statementAudio.gameObject.SetActive(true);
            extra_statementImg.gameObject.SetActive(false);
        }else if (value == (int)QuestionsGroup.InputType.IMAGE)
        {
            extra_statementAudio.gameObject.SetActive(false);
            extra_statementImg.gameObject.SetActive(true);
        }
        else
        {
            extra_statementAudio.gameObject.SetActive(false);
            extra_statementImg.gameObject.SetActive(false);
        }
        questionType.SetValueWithoutNotify(value);
        UpdateCanvas();
    }

    private void OnChangeAlternativeTypeDrop(int value)
    {
        
        if (alternativesGroup != null)
        {
            if (alternativesGroup.HasAnyAlternativeCompleted() > 0)
            {
                QuestionsGroup.Instance.ShowConfirmPanel("Confirma alterar o tipo de alternativas e perder os dados cadastrados?",ChangeAlternativeType,RevertAlternativesType );
            }
            else
            {
                ChangeAlternativeType();
            }
        }
        else
        {
            ChangeAlternativeType();
        }
    }

    public void ChangeAlternativeType()
    {
        if(alternativesGroup != null)
            DestroyImmediate(alternativesGroup.gameObject);
        alternativesGroup =
            Instantiate(
                QuestionsGroup.Instance.GetAlternativeGroupPrefab((QuestionsGroup.InputType)alternativeType.value),
                alternativeGroupContainer);
        previousTypeDropdown = (QuestionsGroup.InputType) alternativeType.value;
        ChangeAlternativeQtt();
        UpdateCanvas();
    }

    private void OnChangeAlternativeQttDrop(int value)
    {
        int qtt = previousQttDropdown;
        int.TryParse(alternativeQtt.options[alternativeQtt.value].text, out qtt);

        int completed = alternativesGroup.HasAnyAlternativeCompleted();
        if (completed > qtt)
        {
            QuestionsGroup.Instance.ShowConfirmPanel(String.Format("Confirma reduzir a quantidade de alternativas e perder {0} alternativas ja preenchidas?", completed-qtt ),ChangeAlternativeQtt,RevertAlternativesQtt );
        }
        else
        {
            ChangeAlternativeQtt();
        }
    }

    public void ChangeAlternativeQtt()
    {  
        int.TryParse(alternativeQtt.options[alternativeQtt.value].text, out previousQttDropdown);
       alternativesGroup.DeactivateAlternatives(previousQttDropdown);
       UpdateCanvas();
    }

    public void RevertAlternativesQtt()
    {
        alternativeQtt.value = GetDropdownIndex(alternativeQtt,previousQttDropdown);
    }
    public void RevertAlternativesType()
    {
        alternativeType.value = (int)previousTypeDropdown;
    }
    
    private int GetDropdownIndex(TMP_Dropdown drop, int qtt)
    {
        return drop.options.FindIndex(x => x.text == qtt.ToString());
    }

    #endregion

    #region FillCheck

    public bool IsQuestionComplete()
    {
        bool isComplete = true;
        if (statementEN_text.InputField.text.IsNullEmptyOrWhitespace())
        {
            statementEN_text.ActivateErrorMode();
            isComplete = false;
        }
        if (statementPT_text.InputField.text.IsNullEmptyOrWhitespace())
        {
            statementPT_text.ActivateErrorMode();
            isComplete = false;
        }
        if (statementPT_audio.UploadedFile == null && !statementPT_audio.IsFilled)
        {
            statementPT_audio.ActivateErrorMode();
            isComplete = false;
        }
        if (statementEN_audio.UploadedFile == null && !statementEN_audio.IsFilled)
        {
            statementEN_audio.ActivateErrorMode();
            isComplete = false;
        }

        if (questionType.value == (int)QuestionsGroup.InputType.AUDIO)
        {
            if (extra_statementAudio.UploadedFile == null && !extra_statementAudio.IsFilled)
            {
                extra_statementAudio.ActivateErrorMode();
                isComplete = false;
            }
        }else if (questionType.value == (int)QuestionsGroup.InputType.IMAGE)
        {
            if (extra_statementImg.UploadedFile == null && !extra_statementImg.IsFilled)
            {
                extra_statementImg.ActivateErrorMode();
                isComplete = false;
            }
        }

        if (!alternativesGroup.IsAllAlternativeCompleted())
        {
            isComplete = false;
        }

        background.sprite = backSprite[Utils.BoolToInt(isComplete)];
        errorSymbol.gameObject.SetActive(!isComplete);
        
        return isComplete;
    }


    public void GetQuestion()
    {
        List<File> files = new List<File>();
        filledUrls = new Dictionary<string, string>();
        
        if (statementEN_audio.UploadedFile == null)
        {
            filledUrls.Add(EN,statementEN_audio.url);
        }else
        {
            files.Add(statementEN_audio.UploadedFile);
        }
        
        if (statementPT_audio.UploadedFile == null)
        {
            filledUrls.Add(PT,statementPT_audio.url);
        }else
        {
            files.Add(statementPT_audio.UploadedFile);
        }

        if (questionType.value == (int)QuestionsGroup.InputType.AUDIO)
        {
            if (extra_statementAudio.UploadedFile == null)
            {
                filledUrls.Add(FILE, extra_statementAudio.url);
            }else
            {
                files.Add(extra_statementAudio.UploadedFile);
            }
        }
        else if (questionType.value == (int)QuestionsGroup.InputType.IMAGE)
        {
            if (extra_statementImg.UploadedFile == null)
            {
                filledUrls.Add(FILE, extra_statementImg.url);
            }else
            {
                files.Add(extra_statementImg.UploadedFile);
            }
        }
        else
        {
            filledUrls.Add(FILE, "");
        }

        files.AddRange(alternativesGroup.GetFiles());
        filledUrls.AddRange(alternativesGroup.FilledImages());
        
        SendFilesPack(files);
    }

    private void SendFilesPack(List<File> fileList)
    {
        RestClient.DefaultRequestHeaders["Authorization"] = "Bearer Luby2021";

        APIFactory.GetApi<FileUpload>().UploadFile(fileList, list =>
        {
            string[] result = list.ToArray();
            
            SendQuestion(result);
        }, error =>
        {
            Debug.LogError(error.message);
            SucessPanel.Instance.SetText("Houve um erro ao enviar os arquivos: "+ error.message,
                SucessPanel.MessageType.ERROR);
        });
    }

    private void SendQuestion(string[] fileList)
    {
        int index = 0;
        string en_audio, pt_audio, statement_url;
        if (filledUrls.ContainsKey(EN))
        {
            en_audio = filledUrls[EN];
        }
        else
        {
            en_audio = fileList[index];
            index++;
        }
        
        if (filledUrls.ContainsKey(PT))
        {
            pt_audio = filledUrls[PT];
        }
        else
        {
            pt_audio = fileList[index];
            index++;
        }
        
        if (filledUrls.ContainsKey(FILE))
        {
            statement_url = filledUrls[FILE];
        }
        else
        {
            statement_url = fileList[index];
            index++;
        }

        List<string> answersUrls = new List<string>();
        foreach (var letter  in alternativesGroup.Letters)
        {
            if (filledUrls.ContainsKey(letter.ToString()))
            {
                answersUrls.Add(filledUrls[letter.ToString()]);
            }
            else
            {
                answersUrls.Add(fileList[index]);
                index++;
            }
        }
        Question question = new Question()
        {
            questionType = (QuestionsGroup.InputType)questionType.value,
            questionTitleEnglish = statementEN_text.InputField.text,
            questionTitlePortuguese = statementPT_text.InputField.text,
            questionTitleAudioEnglish = en_audio,
            questionTitleAudioPortuguese = pt_audio,
            questionFileUrl = statement_url,
            answerType = (QuestionsGroup.InputType) alternativeType.value,
            answers = answersUrls
        };
        
        QuestionsGroup.Instance.ReceiveQuestionData(question, transform.GetSiblingIndex());
    }

    #endregion
    
}

public struct Question
{
    public QuestionsGroup.InputType questionType;
    public string questionTitleEnglish;
    public string questionTitlePortuguese;
    public string questionTitleAudioEnglish;
    public string questionTitleAudioPortuguese;
    public string questionFileUrl;
    public QuestionsGroup.InputType answerType;
    public int correctAnswer;
    public List<string> answers;
}

