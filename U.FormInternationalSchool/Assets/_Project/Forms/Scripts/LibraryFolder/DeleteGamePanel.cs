using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class DeleteGamePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Image coloredPanel;

    [SerializeField] private Image coloredIcons;

    [SerializeField] private Button yesButton;

    [SerializeField] private Button noButton;

    private Action OnYesAction;
    private Action OnNoAction;

    private void Start()
    {
        yesButton.onClick.AddListener(OnYesButton);
        noButton.onClick.AddListener(OnNoButton);
    }

    public void SetPanelColors(Color panel, Color icons)
    {
        coloredPanel.color = panel;
        coloredIcons.color = icons;
    }

    public void SetNewDelete(Action onYes, Action onNo, string title)
    {
        OnYesAction = onYes;
        OnNoAction = onNo;
        messageText.text = String.Format("Tem certeza que deseja deletar o jogo \"{0}\"?", title);
    }

    private void OnNoButton()
    {
        OnNoAction?.Invoke();
        CleanActions();
        this.gameObject.SetActive(false);
    }

    private void OnYesButton()
    {
        OnYesAction?.Invoke();
        CleanActions();
        this.gameObject.SetActive(false);
    }

    private void CleanActions()
    {
        OnYesAction = null;
        OnNoAction = null;
    }

}
