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
        return qtt == HasAnyAlternativeCompleted(true);
    }

    public int HasAnyAlternativeCompleted(bool showError = false)
    {
        int qtt = 0;
        foreach (var alternative in alternatives)
        {
            if (showError)
            {
                if (alternative.IsCompleteWithError())
                {
                    qtt++;
                }
            }
            else
            {
                if (alternative.IsCompleted())
                {
                    qtt++;
                }
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

    public List<string> GetTextAnswers(int qtt)
    {
        List<string> answers = new List<string>();
        for (int i = 0; i < qtt; i++)
        {
            answers.Add(alternatives[i].GetText());
        }

        return answers;
    }
    
    public List<File> GetFiles(int qtt)
    {
        List<File> files = new List<File>();
        for(int i = 0; i < qtt; i++)
        {
            if (alternatives[i].IsCompleted() && !alternatives[i].IsFilled)
            {
                files.Add(alternatives[i].GetFile());
            }
        }
        return files;
    }

    public Dictionary<string,string> FilledImages(int qtt)
    {
        Dictionary<string, string> listFilledImages = new Dictionary<string, string>();
        for(int i = 0; i < qtt; i++)
        {
            if (alternatives[i].IsCompleted() && alternatives[i].IsFilled)
            {
                listFilledImages.Add(alternatives[i].Index.ToString(),alternatives[i].GetFilledUrl());
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

    public int FillAlternativeGroup(List<AnswerGet> answers, int selectedAnswer, FormScreen form, QuestionsGroup.InputType type)
    {
        int count = 0;
        for(int i =0;i < answers.Count; i++)
        {
            if (!answers[i].answer.IsNullEmptyOrWhitespace())
            {
                alternatives[i].FillAlternative(answers[i].answer, i == selectedAnswer, form, type);
                count++;
            }
        }
        DeactivateAlternatives(count);
        return count;
    }
}

