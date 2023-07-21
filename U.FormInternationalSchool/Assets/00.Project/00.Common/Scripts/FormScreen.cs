using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private TMP_InputField statement_PT, statement_EN;
    [SerializeField] private Toggle timer;
    [SerializeField] private TMP_InputField time;
    [SerializeField] private Button up, down;
    [SerializeField] private TMP_InputField timerBonus;

    private AudioClip music;
    private AudioClip statementAudio_PT, statementAudio_EN;
    private Sprite titleImg;
    private Sprite background;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
