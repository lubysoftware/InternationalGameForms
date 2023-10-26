using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Settings Buttons")]
    [SerializeField] private Button moveUpperBtn;
    [SerializeField] private Button moveDownBtn;
    [SerializeField] private Button deleteQuestion;
    [SerializeField] private GameObject panelSetting;
    [SerializeField] private Button settingsButton;

    [Header("Question error view")] 
    [SerializeField] private Image background;
    [SerializeField]  GameObject errorSymbol;
    [SerializeField] private Sprite[] backSprite;

    private int previousQttDropdown;

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
            transform.SetSiblingIndex(index-1);
        }
    }

    private void ShowPanel()
    {
        panelSetting.gameObject.SetActive(!panelSetting.activeInHierarchy);
    }
    private void MoveDown()
    {  
        int index = transform.GetSiblingIndex();
        if (index < QuestionsGroup.Instance.QuestionsQtt - 1)
        {
            transform.SetSiblingIndex(index+1);
        }
    }

    private void DeleteButton()
    {
        // Confirm panel
        Destroy(this.gameObject);
        QuestionsGroup.Instance.CheckAddQuestionButton();
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
    }

    private void OnChangeAlternativeTypeDrop(int value)
    {
        if (alternativesGroup != null)
        {
            if (alternativesGroup.HasAnyAlternativeCompleted() > 0)
            {
                // confirm popup
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
        alternativesGroup = Instantiate(QuestionsGroup.Instance.GetAlternativeGroupPrefab((QuestionsGroup.InputType)alternativeType.value), alternativeGroupContainer);
        alternativeQtt.value = 3;
    }

    private void OnChangeAlternativeQttDrop(int value)
    {
        int qtt = previousQttDropdown;
        int.TryParse(alternativeQtt.options[alternativeQtt.value].text, out qtt);
        
        if (alternativesGroup.HasAnyAlternativeCompleted() > qtt)
        {
            // confirm popup
        }
        else
        {
            previousQttDropdown = qtt;
            ChangeAlternativeQtt();
        }
    }

    public void ChangeAlternativeQtt()
    {
       alternativesGroup.DeactivateAlternatives(previousQttDropdown);
    }

    public void RevertAlternativesQtt()
    {
        alternativeQtt.value = GetDropdownIndex(alternativeQtt,previousQttDropdown);
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

    private Question GetQuestion()
    {
        return new Question()
        {
            questionType = (QuestionsGroup.InputType)questionType.value,
            questionTitleEnglish = statementEN_text.InputField.text,
            questionTitlePortuguese = statementPT_text.InputField.text,
            // upload all files 
        };
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
    public QuizAlternative[] answers;
}

