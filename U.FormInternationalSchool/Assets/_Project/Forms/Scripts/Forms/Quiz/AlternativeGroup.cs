using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlternativeGroup : MonoBehaviour
{
    [SerializeField] private QuestionsGroup.InputType type;
    [SerializeField] private List<QuizAlternative> alternatives;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsAllAlternativeCompleted()
    {
        return false;
    }

    public int HasAnyAlternativeCompleted()
    {
        return 0;
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
            }
            else
            {
                if (alternatives[i].Deactivate())
                {
                    alternatives[0].ActivateToggle();
                }
            }
        }
        
        
    }
}

