using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsPanel : MonoBehaviour
{
    [Header("COLORS")]
    
    [Header("color panel")]
    [SerializeField] private Image coloredPanel;
    [SerializeField] private Image coloredIcons;

    [Header("color - label backgrounds")]
    [SerializeField] private Image coloredBackLabelTimer;
    [SerializeField] private Image coloredBackLabelBonus;
    [SerializeField] private Image coloredBackLabelError;
    
    [Header("color - countors")]
    [SerializeField] private Image coloredLineLabelTimer;
    [SerializeField] private Image coloredLineLabelBonus;
    [SerializeField] private Image coloredLineLabelError;
    
    [Header("color - symbols")]
    [SerializeField] private Image coloredSymbolTimer;
    [SerializeField] private Image coloredSymbolBonus;
    [SerializeField] private Image coloredSymbolError;
    
    [Space(15)]
    [Header("timer")]
    [SerializeField] private TMP_InputField inputTimerMin;
    [SerializeField] private TMP_InputField inputTimerSec;
    [SerializeField] private Toggle toggleTimer;
    
    [Header("bonus")]
    [SerializeField] private TMP_InputField inputBonus;
    [SerializeField] private Toggle toggleBonus;
    
    [Header("error")]
    [SerializeField] private TMP_InputField inputError;
    [SerializeField] private Toggle toggleError;
    
    [Header("general")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;
    
    void Start()
    {
        
    }

    public void SetPanelColors(Color darkColor, Color lightColor, Color defaultColor, Color secondaryColor, Color SymbolColor )
    {
        coloredPanel.color = darkColor;
        coloredIcons.color = lightColor;
        coloredBackLabelTimer.color = coloredBackLabelError.color = coloredBackLabelBonus.color = defaultColor;
        coloredLineLabelTimer.color = coloredLineLabelError.color = coloredLineLabelBonus.color = darkColor;
        toggleBonus.targetGraphic.color = toggleError.targetGraphic.color = toggleTimer.targetGraphic.color = defaultColor;
        toggleBonus.graphic.color = toggleError.graphic.color = toggleTimer.graphic.color = darkColor;
        inputTimerMin.targetGraphic.color = inputTimerSec.targetGraphic.color =
            inputBonus.targetGraphic.color = inputError.targetGraphic.color = darkColor;
        coloredSymbolBonus.color = coloredSymbolError.color = coloredSymbolTimer.color = SymbolColor;
        resetButton.image.color = secondaryColor;
        closeButton.image.color = darkColor;
    }
}
