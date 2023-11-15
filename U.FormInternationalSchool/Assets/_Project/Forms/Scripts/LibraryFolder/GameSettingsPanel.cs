using System;
using System.Collections;
using System.Collections.Generic;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameSettingsPanel : MonoBehaviour
{
    [Header("COLORS")] [Header("color panel")] [SerializeField]
    private Image coloredPanel;

    [SerializeField] private Image coloredIcons;

    [Header("color - label backgrounds")] [SerializeField]
    private Image coloredBackLabelTimer;

    [SerializeField] private Image coloredBackLabelBonus;
    [SerializeField] private Image coloredBackLabelError;

    [Header("color - countors")] [SerializeField]
    private Image coloredLineLabelTimer;

    [SerializeField] private Image coloredLineLabelBonus;
    [SerializeField] private Image coloredLineLabelError;

    [Header("color - symbols")] [SerializeField]
    private Image coloredSymbolTimer;

    [SerializeField] private Image coloredSymbolBonus;
    [SerializeField] private Image coloredSymbolError;

    [Space(15)] [Header("timer")] [SerializeField]
    private TMP_InputField inputTimerMin;

    [SerializeField] private TMP_InputField inputTimerSec;
    [SerializeField] private Toggle toggleTimer;
    [SerializeField] private GameObject overlayTimer;
    [SerializeField] private GameObject timerMessage;

    [Header("bonus")] [SerializeField] private TMP_InputField inputBonus;
    [SerializeField] private Toggle toggleBonus;
    [SerializeField] private GameObject overlayBonus;
    [SerializeField] private GameObject bonusMessage;

    [Header("error")] [SerializeField] private TMP_InputField inputError;
    [SerializeField] private Toggle toggleError;
    [SerializeField] private GameObject overlayError;
    [SerializeField] private GameObject errorMessage;

    [FormerlySerializedAs("resetButton")] [Header("general")] [SerializeField]
    private Button saveButton;

    [SerializeField] private Button closeButton;

    [SerializeField] private LibraryScreen library;

    private Color normalColor;

    private bool hasErrorPoints;

    private GameType gameType;
    

    void Start()
    {
        saveButton.onClick.AddListener(SaveButton);
        closeButton.onClick.AddListener(CloseButton);
    }

    public void OnEnable()
    {
       FillData();
    }

    public void FillData()
    {
        DefaultSettings settings;
        SceneDataCarrier.GetData(Constants.GAME_SETTINGS, out settings);

        if (settings != null)
        {
            if (settings.timer > 0)
            {
                int min = settings.timer / 60;
                int sec = settings.timer - min * 60;
                inputTimerMin.text = String.Format("{0:00}", min);
                inputTimerSec.text = String.Format("{0:00}", sec);
            }

            if (settings.bonusTimer >= 0)
            {
            }

            inputBonus.text = settings.bonusTimer.ToString();
            if (settings.mistakePoints >= 0)
                inputError.text = settings.mistakePoints.ToString();

            toggleError.isOn = settings.hasMistakePoints;
            toggleBonus.isOn = settings.hasBonusTime;
            toggleTimer.isOn = settings.hasTimer;
        }

    }

    public void SetPanelColors(Color darkColor, Color lightColor, Color defaultColor, Color secondaryColor,
        Color SymbolColor, GameType type)
    {
        coloredPanel.color = darkColor;
        coloredIcons.color = lightColor;
        coloredBackLabelTimer.color = coloredBackLabelError.color = coloredBackLabelBonus.color = defaultColor;
        coloredLineLabelTimer.color = coloredLineLabelError.color = coloredLineLabelBonus.color = darkColor;
        toggleBonus.targetGraphic.color =
            toggleError.targetGraphic.color = toggleTimer.targetGraphic.color = defaultColor;
        toggleBonus.graphic.color = toggleError.graphic.color = toggleTimer.graphic.color = darkColor;
        inputTimerMin.targetGraphic.color = inputTimerSec.targetGraphic.color =
            inputBonus.targetGraphic.color = inputError.targetGraphic.color = darkColor;
        coloredSymbolBonus.color = coloredSymbolError.color = coloredSymbolTimer.color = SymbolColor;
        saveButton.image.color = secondaryColor;
        closeButton.image.color = darkColor;

        normalColor = darkColor;

        gameType = type;
        hasErrorPoints = HasErrorPoints();
        
    }

    public void CloseButton()
    {
        gameObject.SetActive(false);
    }

    protected int CalculateTimeInSec()
    {
        int min, sec = 0;
        int timeTotal = 0;
        int.TryParse(inputTimerMin.text, out min);
        int.TryParse(inputTimerSec.text, out sec);

        timeTotal = min * 60 + sec;
        if (timeTotal <= 0)
        {
            if (toggleTimer.isOn)
            {
                inputTimerMin.targetGraphic.color = inputTimerSec.targetGraphic.color = Color.red;
                timerMessage.gameObject.SetActive(true);
            }
            return -1;
        }
        
        return timeTotal;
    }

    public void ResetButton()
    {
        inputTimerMin.text = string.Empty;
        inputTimerSec.text = string.Empty;
        inputBonus.text = string.Empty;
        inputError.text = string.Empty;
    }

    public void SaveButton()
    {
        bool hasErrorMessage = false;

        int time = -1;
        if (inputTimerMin.text.IsNullEmptyOrWhitespace() || inputTimerSec.text.IsNullEmptyOrWhitespace())
        {
            Debug.LogError("timer null");
            toggleTimer.SetIsOnWithoutNotify(false);
        }
        else
        {
            time = CalculateTimeInSec();
            if (time < 0)
            {
                hasErrorMessage = true;
            }
        }

        int bonusTime = -1;
        if (inputBonus.text.IsNullEmptyOrWhitespace())
        {
            Debug.LogError("bonus null");
            toggleBonus.SetIsOnWithoutNotify(false);
        }
        else
        {
            int.TryParse(inputBonus.text, out bonusTime);
            if (toggleBonus.isOn)
            {
                bonusTime = CheckBonus();
                if (bonusTime < 0)
                {
                    hasErrorMessage = true;
                }
            }
        }

        int errorPoints = -1;
        if (hasErrorPoints)
        {
            if (inputError.text.IsNullEmptyOrWhitespace())
            {
                Debug.LogError("error null");
                toggleError.SetIsOnWithoutNotify(false);
            }
            else
            {
                int.TryParse(inputError.text, out errorPoints);
                if (toggleError.isOn)
                {
                    errorPoints = CheckError();
                    if (errorPoints < 0)
                    {
                        hasErrorMessage = true;
                    }
                }
            }

        }

        if (!hasErrorMessage)
        {
            DefaultSettings settings = new DefaultSettings()
            {
                hasTimer = toggleTimer.isOn,
                hasBonusTime = toggleBonus.isOn,
                hasMistakePoints = toggleError.isOn,
                timer = time,
                bonusTimer = bonusTime,
                mistakePoints = errorPoints,
                gameType = gameType.ToString()
            };
            string json = JsonConvert.SerializeObject(settings);

            APICommunication.Instance.StartUploadDefaultSettings(json, this, library);
        }
    }

    public void OnDisable()
    {
        ResetButton();
    }

    public void CheckBonusInput()
    {
        CheckBonus();
    }

    private int CheckBonus()
    {
        int value = 0;
        int.TryParse(inputBonus.text, out value);
        if (value < 0 || value > 100)
        {
            inputBonus.targetGraphic.color = Color.red;
            bonusMessage.gameObject.SetActive(true);
            return -1;
        }

        return value;
    }
    public void CheckErrorInput()
    {
        CheckError();
    }
    private int CheckError()
    {
        int value = 0;
        int.TryParse(inputError.text, out value);
        if (value < 0)
        {
            inputError.targetGraphic.color = Color.red;
            errorMessage.gameObject.SetActive(true);
            return -1;
        }

        return value;
    }

    public void BackToNormalField(string type)
    {
        switch (type)
        {
            case "timer":
                inputTimerMin.targetGraphic.color = normalColor;
                inputTimerSec.targetGraphic.color = normalColor;
                timerMessage.gameObject.SetActive(false);
                break;
            case "bonus":
                inputBonus.targetGraphic.color = normalColor;
                bonusMessage.gameObject.SetActive(false);
                break;
            case "error":
                inputError.targetGraphic.color = normalColor;
                errorMessage.gameObject.SetActive(false);
                break;
        }
    }

    public bool HasErrorPoints()
    {
        if (gameType == GameType.PUZZLE || gameType == GameType.CROSS_WORD || gameType == GameType.MATCH_CARD ||
            gameType == GameType.QUIZ)
        {
            Debug.Log("without error");
            overlayError.SetActive(true);
            toggleError.SetIsOnWithoutNotify(false);
            toggleError.interactable = false;
            return false;
        }

        return true;
    }

    public void DeactivateField(string field)
    {
        switch (field)
        {
            case "timer":
                overlayTimer.gameObject.SetActive(!toggleTimer.isOn);
                break;
            case "bonus":
                overlayBonus.gameObject.SetActive(!toggleBonus.isOn);
                break;
            case "error":
                if (hasErrorPoints)
                    overlayError.gameObject.SetActive(!toggleError.isOn);
                break;
        }
    }

    public void CheckTimerInput()
    {
        int valueMin = 0;
        int valueSec = 0;
        int.TryParse(inputTimerMin.text, out valueMin);
        int.TryParse(inputTimerSec.text, out valueSec);
        if (valueMin < 0)
        {
            inputTimerMin.targetGraphic.color = Color.red;
            timerMessage.gameObject.SetActive(true);
        }else if (valueSec < 0 || valueSec > 59)
        {
            inputTimerSec.targetGraphic.color = Color.red;
            timerMessage.gameObject.SetActive(true);
        }
        else
        {
            BackToNormalField("timer");
        }
    }


}

public class DefaultSettings
{
    public string gameType;
    public int timer;
    public bool hasTimer;
    public int bonusTimer;
    public bool hasBonusTime;
    public int mistakePoints;
    public bool hasMistakePoints;
}