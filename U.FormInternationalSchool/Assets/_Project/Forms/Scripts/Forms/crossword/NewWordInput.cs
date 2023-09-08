using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing.Drawing2D;
using LubyLib.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

public class NewWordInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField wordText;

    [SerializeField] private Toggle isHorizontalToggle;

    [SerializeField] private TMP_InputField tipText;

    [SerializeField] private ImageFrame image;

    [SerializeField] private TMP_Dropdown rowDrop;

    [SerializeField] private TMP_Dropdown columnDrop;

    [SerializeField] private TextMeshProUGUI indexText;

    [SerializeField] private Button checkButton;

    [SerializeField] private Button cancelButton;

    [SerializeField] private Button editButton;

    [SerializeField] private Button deleteButton;
    public int Index;

    public event Action<NewWordInput, bool> OnDelete;

    public WordInfo Info;
    public struct WordInfo
    {
        public CellItem.Coord Coord;
        public bool IsHorizontal;
        public string Word;
    }
    

    void Start()
    {
        checkButton.onClick.AddListener(CheckWord);
        deleteButton.onClick.AddListener(OnDeleteButton);
        editButton.onClick.AddListener(OnEditButton);
        cancelButton.onClick.AddListener(CancelEdit);
        SetInteractable(false);
    }

    private void CheckWord()
    {
        wordText.text = wordText.text.TrimStart();
        wordText.text = wordText.text.TrimEnd();
        if (wordText.text.Length > 10)
        {
            CrosswordPanel.Instance.SetError(CrosswordPanel.CrosswordError.BIG, null);
        }
        else
        {
            CheckWord(0);
        }
    }
    
    private void CheckWord(int value)
    {
        if (!wordText.text.IsNullEmptyOrWhitespace())
        {
            char dropRow = '-';
            char.TryParse(rowDrop.options[rowDrop.value].text, out dropRow);
            int dropColumn = 0;
            int.TryParse(columnDrop.options[columnDrop.value].text, out dropColumn);
            if (dropRow != '-' && dropColumn != 0)
            {
                CellItem.Coord coord = new CellItem.Coord()
                {
                    row = dropRow,
                    column = dropColumn
                };
                WordInfo newInfo = new WordInfo()
                {
                    Coord = coord,
                    Word = wordText.text,
                    IsHorizontal = isHorizontalToggle.isOn
                };
                if(CrosswordPanel.Instance.CheckWord(newInfo, Info.Word, Index))
                {
                    Info.Coord = coord;
                    Info.Word = wordText.text;
                    Info.IsHorizontal = isHorizontalToggle.isOn;
                    SetInteractable(false);
                    CrosswordPanel.Instance.CancelEditing(this);
                }
            }
        }
    }

    private void CancelEdit()
    {
        ResetData();
        SetInteractable(false);
        CrosswordPanel.Instance.CancelEditing(this);
    }

    private void SetData(WordInfo info)
    {
        rowDrop.SetValueWithoutNotify( rowDrop.options.FindIndex(x => x.text == info.Coord.row.ToString()));
        columnDrop.SetValueWithoutNotify( columnDrop.options.FindIndex(x => x.text == info.Coord.column.ToString()));
        wordText.text = info.Word;
        isHorizontalToggle.SetIsOnWithoutNotify(info.IsHorizontal);
        Info.Coord = info.Coord;
        Info.Word = info.Word;
        Info.IsHorizontal = info.IsHorizontal;
    }

    public void SetCoords(CellItem.Coord coord)
    {
        rowDrop.SetValueWithoutNotify( rowDrop.options.FindIndex(x => x.text == coord.row.ToString()));
        columnDrop.SetValueWithoutNotify( columnDrop.options.FindIndex(x => x.text == coord.column.ToString()));
    }

    private void ResetData()
    {
        SetData(Info);
    }

    public void Init(int index, WordInfo info)
    {
        SetIndex(index);
        
        SetData(info);
    }

    public void SetIndex(int index)
    {
        Index = index;
        indexText.text = "Palavra " + index;
    }

    private void OnDeleteButton()
    {
        OnDelete?.Invoke(this, true);
    }
    
    private void SetInteractable(bool status)
    {
        wordText.interactable = status;
        rowDrop.interactable = status;
        columnDrop.interactable = status;
        isHorizontalToggle.interactable = status;
        isHorizontalToggle.graphic.color = status? new Color(1, 1, 1, 1f) : new Color(0.78f, 0.78f, 0.78f, 0.5f);
        if (isHorizontalToggle.isOn)
        {
            isHorizontalToggle.targetGraphic.color = status? new Color(1, 1, 1, 1f) : new Color(1, 1, 1, 0f);
        }
        else
        {
            isHorizontalToggle.targetGraphic.color = status? new Color(1, 1, 1, 1f) : new Color(0.78f, 0.78f, 0.78f, 0.5f);
        }
        checkButton.gameObject.SetActive(status);
        cancelButton.gameObject.SetActive(status);
        editButton.gameObject.SetActive(!status);
        deleteButton.gameObject.SetActive(!status);
    }

    private void OnEditButton()
    {
        SetInteractable(true);
        CrosswordPanel.Instance.SetEdit(this);
    }
    public void ChangeType(bool isImage)
    {
        tipText.gameObject.SetActive(!isImage);
        image.gameObject.SetActive(isImage);
    }
}
