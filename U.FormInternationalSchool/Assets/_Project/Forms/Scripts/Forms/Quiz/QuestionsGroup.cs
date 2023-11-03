using System;
using System.Collections;
using System.Collections.Generic;
using LubyLib.Core.Singletons;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestionsGroup : SimpleSingleton<QuestionsGroup>
{
    [Header("Prefabs")] [SerializeField] private AlternativeGroup textGroupPrefab;
    [SerializeField] private AlternativeGroup audioGroupPrefab;
    [SerializeField] private AlternativeGroup imgGroupPrefab;
    [SerializeField] private QuestionManager questionPrefab;

    [Header("Inputs")] [SerializeField] private Button addQuestion;
    [SerializeField] private TMP_Dropdown questionType;

    [Header("Confirm Panel")] [SerializeField]
    private Transform confirmReducePanel;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;

    [Space(15)] private Question[] questionsData;


    [SerializeField] private Transform questionsContainer;

    [Space(15)] [Header("Layouts")] [SerializeField]
    private LayoutGroup layout;

    [SerializeField] private LayoutGroup layout2;
    [SerializeField] private LayoutGroup layout3;

    [SerializeField] private QuizForm form;

    private int receivedData = 0;
    public int QuestionsQtt => questionsContainer.childCount;

    public enum InputType
    {
        TEXT,
        AUDIO,
        IMAGE
    }

    void Start()
    {
        addQuestion.onClick.AddListener(OnAddQuestion);
        UpdateCanvas();
    }

    public AlternativeGroup GetAlternativeGroupPrefab(InputType type)
    {
        switch (type)
        {
            case InputType.TEXT:
                return textGroupPrefab;
            case InputType.AUDIO:
                return audioGroupPrefab;
            case InputType.IMAGE:
                return imgGroupPrefab;
        }

        return textGroupPrefab;
    }

    public void OnAddQuestion()
    {
        QuestionManager newQuestion = Instantiate(questionPrefab, questionsContainer);
        newQuestion.SetQuestionType(questionType.value);
        UpdateCanvas();
        CheckAddQuestionButton();
    }

    public void CheckAddQuestionButton()
    {
        addQuestion.interactable = transform.childCount < 10;
        form.UpdatePoints();
    }

    public void ShowConfirmPanel(string errorMessage, UnityAction yes, UnityAction cancel)
    {
        alertMessage.text = errorMessage;
        SetConfirmPanelListeners(yes, cancel);
        confirmReducePanel.gameObject.SetActive(true);
    }

    private void SetConfirmPanelListeners(UnityAction confirm, UnityAction deny)
    {
        confirmButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirm);
        denyButton.onClick.AddListener(deny);
        denyButton.onClick.AddListener(() => confirmReducePanel.gameObject.SetActive(false));
    }

    public void UpdateCanvas()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout2.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout3.transform as RectTransform);
    }

    public void UpdateAllQuestionsIndex()
    {
        foreach(Transform child in questionsContainer)
        {
            child.GetComponent<QuestionManager>().UpdateIndex();
        }
    }

    public bool CheckAllQuestions()
    {
        bool isCompleted = true;
        foreach(Transform child in questionsContainer)
        {
            if (!child.GetComponent<QuestionManager>().IsQuestionComplete())
            {
                isCompleted = false;
            }
        }

        return isCompleted;
    }

    public void GetAllQuestionData()
    {
        receivedData = 0;
        questionsData = new Question[QuestionsQtt];
        foreach(Transform child in questionsContainer)
        {
            child.GetComponent<QuestionManager>().GetQuestion();
        }
    }

    public void ReceiveQuestionData(Question question, int index)
    {
        receivedData++;
        questionsData[index] = question;
        if (receivedData == QuestionsQtt)
        {
            form.SerializeGameData(questionsData);
        }
    }

    public void FillQuestions(QuestionGet[] questions)
    {
        foreach (QuestionGet q in questions)
        {
            QuestionManager newQuestion = Instantiate(questionPrefab, questionsContainer);
            newQuestion.FillQuestionData(q, form);
        }
        UpdateCanvas();
        CheckAddQuestionButton();
    }

    public void DeactivateErrorMode(InputElement el)
    {
        form.DeactivateErrorInput(el);
    }

    public bool IsEdit()
    {
        return form.isEdit;
    }
    

}

[Serializable]
public class Quiz
{
    public int failPenalty;
    public bool randomAnswers;
    public Question[] questions;
}