using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizAlternative : MonoBehaviour
{
    public int Index;

    [SerializeField] private Toggle isCorrect;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsCompleted()
    {
        return true;
    }

    public bool Deactivate()
    {
        gameObject.SetActive(false);
        bool correct = isCorrect.isOn;
        isCorrect.isOn = false;
        return correct;
    }

    public void ActivateToggle()
    {
        isCorrect.isOn = true;
    }
}
