using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing.Drawing2D;
using LubyLib.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    public event Action<NewWordInput> OnDelete;

    public CellItem.Coord c;
    public bool isHorizontal;
    public string word;

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
                if(CrosswordPanel.Instance.CheckWord(coord,wordText.text,isHorizontalToggle.isOn, word, Index))
                {
                    c = coord;
                    word = wordText.text;
                    isHorizontal = isHorizontalToggle.isOn;
                    SetInteractable(false);
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

    private void SetData(CellItem.Coord coord, string word, bool isHorizontal)
    {
        rowDrop.SetValueWithoutNotify( rowDrop.options.FindIndex(x => x.text == coord.row.ToString()));
        columnDrop.SetValueWithoutNotify( columnDrop.options.FindIndex(x => x.text == coord.column.ToString()));
        wordText.text = word;
        isHorizontalToggle.SetIsOnWithoutNotify(isHorizontal);
        c = coord;
        this.word = word;
        this.isHorizontal = isHorizontal;
    }

    public void SetCoords(CellItem.Coord coord)
    {
        rowDrop.SetValueWithoutNotify( rowDrop.options.FindIndex(x => x.text == coord.row.ToString()));
        columnDrop.SetValueWithoutNotify( columnDrop.options.FindIndex(x => x.text == coord.column.ToString()));
    }

    private void ResetData()
    {
        SetData(c,word,isHorizontal);
    }

    public void Init(int index, CellItem.Coord coord, string word, bool isHorizontal)
    {
        SetIndex(index);
        SetData(coord,word,isHorizontal);
    }

    public void SetIndex(int index)
    {
        Index = index;
        indexText.text = "Palavra " + index;
    }

    private void OnDeleteButton()
    {
        OnDelete?.Invoke(this);
    }
    
    private void SetInteractable(bool status)
    {
        wordText.interactable = status;
        rowDrop.interactable = status;
        columnDrop.interactable = status;
        isHorizontalToggle.interactable = status;
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
