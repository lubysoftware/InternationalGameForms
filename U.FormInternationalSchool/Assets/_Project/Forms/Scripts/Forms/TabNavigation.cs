using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabNavigation : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFieldList;

    private int currentIndex;

    private void Start()
    {
        currentIndex = 0;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PreviousInput();
            }
            else
            {
                NextInput();
            }
        }
    }

    private void NextInput()
    {
        if (currentIndex + 1 < inputFieldList.Length)
        {
            currentIndex++;
            if (inputFieldList[currentIndex].isActiveAndEnabled)
            {
                inputFieldList[currentIndex].Select();
            }
            else
            {
                NextInput();
            }
            
        }
    }
    
    private void PreviousInput()
    {
        if (currentIndex - 1 >= 0)
        {
            currentIndex--;
            if (inputFieldList[currentIndex].isActiveAndEnabled)
            {
                inputFieldList[currentIndex].Select();
            }
            else
            {
                PreviousInput();
            }
        }
    }

    public void SetIndex(int index)
    {
        currentIndex = index;
    }
}
