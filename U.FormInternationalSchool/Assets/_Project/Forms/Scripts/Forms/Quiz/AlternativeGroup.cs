using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AlternativeGroup : MonoBehaviour
{
    [SerializeField] private QuestionsGroup.InputType type;
    [SerializeField] private List<QuizAlternative> alternatives;

    public char[] Letters = { 'A', 'B', 'C', 'D'};
    
    void Start()
    {
        
    }

    public bool IsAllAlternativeCompleted(int qtt)
    {
        return qtt == HasAnyAlternativeCompleted();
    }

    public int HasAnyAlternativeCompleted()
    {
        int qtt = 0;
        foreach (var alternative in alternatives)
        {
            if (alternative.IsCompleted())
            {
                qtt++;
            }
        }
        return qtt;
    }
    

    public void DeactivateAlternatives(int qtt)
    {
        for(int i = 0; i < alternatives.Count; i++)
        {
            if (!alternatives[i].IsCompleted())
            {
                alternatives[i].transform.SetAsLastSibling();
            }
        }
        alternatives.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        for (int i = 0; i < alternatives.Count; i++)
        {
            if (i < qtt )
            {
                alternatives[i].gameObject.SetActive(true);
                alternatives[i].SetIndex(Letters[i]);
            }
            else
            {
                if (alternatives[i].Deactivate())
                {
                    alternatives[0].ActivateToggle();
                }
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<LayoutGroup>().transform as RectTransform);
    }

    public List<string> GetTextAnswers()
    {
        List<string> answers = new List<string>();
        foreach (var item in alternatives)
        {
            answers.Add(item.GetText());
        }

        return answers;
    }
    
    public List<File> GetFiles()
    {
        List<File> files = new List<File>();
        foreach (var item in alternatives)
        {
            if (item.IsCompleted() && !item.IsFilled)
            {
                files.Add(item.GetFile());
            }
        }
        return files;
    }

    public Dictionary<string,string> FilledImages()
    {
        Dictionary<string, string> listFilledImages = new Dictionary<string, string>();
        foreach (var item in alternatives)
        {
            if (item.IsCompleted() && item.IsFilled)
            {
                listFilledImages.Add(item.Index.ToString(),item.GetFilledUrl());
            }
        }

        return listFilledImages;
    }

    public int GetCorrectAnswer()
    {
        foreach (var item in alternatives)
        {
            if (item.IsCorrect)
            {
                return item.transform.GetSiblingIndex();
            }
        }

        return 0;
    }

    public int FillAlternativeGroup(List<AnswerGet> answers, int selectedAnswer, FormScreen form)
    {
        int count = 0;
        for(int i =0;i < alternatives.Count; i++)
        {
            if (!answers[i].answer.IsNullEmptyOrWhitespace())
            {
                alternatives[i].FillAlternative(answers[i].answer, i == selectedAnswer, form);
                count++;
            }
        }
        Debug.LogError(count);
        DeactivateAlternatives(count);
        return count;
    }
}

