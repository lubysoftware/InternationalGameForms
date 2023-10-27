using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlternativeGroup : MonoBehaviour
{
    [SerializeField] private QuestionsGroup.InputType type;
    [SerializeField] private List<QuizAlternative> alternatives;
    
    private char[] letters = { 'A', 'B', 'C', 'D'};
    
    void Start()
    {
        
    }

    public bool IsAllAlternativeCompleted()
    {
        return alternatives.Count == HasAnyAlternativeCompleted();
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
                alternatives[i].SetIndex(letters[i]);
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
}

