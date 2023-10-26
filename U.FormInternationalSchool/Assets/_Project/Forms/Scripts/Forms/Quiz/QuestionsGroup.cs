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
    [Header("Prefabs")]
    [SerializeField] private AlternativeGroup textGroupPrefab;
    [SerializeField] private AlternativeGroup audioGroupPrefab;
    [SerializeField] private AlternativeGroup imgGroupPrefab;
    [SerializeField] private QuestionManager questionPrefab;
    
    [Header("Inputs")]
    [SerializeField] private Button addQuestion;
    [SerializeField] private TMP_Dropdown questionType;
    [SerializeField] private Toggle randomize;
    
    [Header("Confirm Panel")]
    [SerializeField] private Transform confirmReducePanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;

    private QuestionManager[] questions;

    
    [SerializeField] private Transform questionsContainer;
    [SerializeField] private Transform addinfoTransform;
    public int QuestionsQtt => transform.childCount;

    public enum InputType
    {
        TEXT,
        AUDIO,
        IMAGE
    }
    
    void Start()
    {
        addQuestion.onClick.AddListener(OnAddQuestion);
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
        addinfoTransform.SetAsLastSibling();
        CheckAddQuestionButton();
    }

    public void CheckAddQuestionButton()
    {
        addQuestion.interactable = transform.childCount < 10;
    }

    public void ShowConfirmPanel(string errorMessage, UnityAction yes, UnityAction cancel)
    {
        alertMessage.text =errorMessage;
        SetConfirmPanelListeners(yes,cancel);
    }
    
    private void SetConfirmPanelListeners(UnityAction confirm, UnityAction deny)
    {
        confirmButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(confirm);
        denyButton.onClick.AddListener(deny);
    }
    
    
}
