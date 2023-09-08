using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using LubyLib.Core.Singletons;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Toggle = UnityEngine.UI.Toggle;

public class CrosswordPanel : SimpleSingleton<CrosswordPanel>
{

    [SerializeField] private FormScreen form;
    [SerializeField] private WordsGrid grid;
    [SerializeField] private NewWordInput[] wordInput;
    [SerializeField] private Button newWordButton;
    [SerializeField] private Transform wordsContainer;
    [SerializeField] private Toggle isImage;
    [SerializeField] private int[] maxItems;
    
    [SerializeField] private Transform confirmChangePanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;
    
    [SerializeField] private TMP_Dropdown row;
    [SerializeField] private TMP_Dropdown column;
    [SerializeField] private TMP_InputField wordText;
    [SerializeField] private Toggle isHorizontalToggle;

    [SerializeField] private LayoutGroup layout;
    [SerializeField] private LayoutGroup layout2;
    [SerializeField] private GridLayoutGroup gridLayout;
   
    private List<NewWordInput> wordInputs;
    private List<NewWordInput.WordInfo> wordInfos;

    private NewWordInput editingInput = null;

    public enum CrosswordError
    {
        FIT,
        MATCH, 
        BIG,
        EMPTY
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(ChangeTipType);
        denyButton.onClick.AddListener(ResetToggle);
        newWordButton.onClick.AddListener(InsertNewWord);
        isImage.onValueChanged.AddListener(OnChangeTipType);
        wordInputs = new List<NewWordInput>();
        wordInfos = new List<NewWordInput.WordInfo>();
    }

    public bool CheckWord(NewWordInput.WordInfo info, string previousWord, int index)
    {
        if (info.Word.Length > 10)
        {
            SetError(CrosswordError.BIG, null);
            return false;
        }

        if (!grid.CheckIfFitOnGrid(info))
        {
            SetError(CrosswordError.FIT, null);
            return false;
        }
        if(!grid.CheckInterval(info, previousWord))
        {
            SetError(CrosswordError.MATCH, null);
            return false;
        }

        if (!previousWord.IsNullEmptyOrWhitespace())
        {
            NewWordInput previousInput = wordInputs.Find(x => index == x.Index);
            grid.ClearInterval(previousInput.Info);
        }
        
        grid.FillInterval(info, index);
        
        return true;
    }

    public void SetError(CrosswordError type, int[] values)
    {
        string error = "error";
        switch (type)
        {
            case CrosswordError.FIT:
                error = "A Palavra excede os limites do grid. Corrija a posição inicial.";
                break;
            case CrosswordError.BIG:
                error = String.Format("A Palavra deve conter até {0} letras.", 10);
                break;
            case CrosswordError.MATCH:
                error = "A Palavra não é compatível com as palavras já inseridas no grid.";
                break;
            case CrosswordError.EMPTY:
                error = "A Palavra não pode ser vazia.";
                break;
        }

        form.ShowError(error, ErrorType.CUSTOM, null);
    }

    private void InsertNewWord()
    {
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];
        wordText.text = wordText.text.TrimStart();
        wordText.text = wordText.text.TrimEnd();
        if (wordText.text.IsNullEmptyOrWhitespace())
        {
            SetError(CrosswordError.EMPTY, null);
            return;
        }
        if (wordInputs.Count < max)
        {
            char dropRow = '-';
            char.TryParse(row.options[row.value].text, out dropRow);
            int dropColumn = 0;
            int.TryParse(column.options[column.value].text, out dropColumn);
            if (dropRow != '-' && dropColumn != 0)
            {
                CellItem.Coord coord = new CellItem.Coord()
                {
                    row = dropRow,
                    column = dropColumn
                };

                NewWordInput.WordInfo info = new NewWordInput.WordInfo()
                {
                    Word = wordText.text,
                    Coord = coord,
                    IsHorizontal = isHorizontalToggle.isOn
                };
                CreateNewWord(info, true);   
            }
            
        }
    }

    private void CreateNewWord(NewWordInput.WordInfo info, bool addInfo)
    {
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];
        if(CheckWord(info, null, wordInputs.Count +1))
        {
            NewWordInput input = Instantiate(wordInput[Utils.BoolToInt(isImage.isOn)], wordsContainer);
            wordInputs.Add(input);
            if (addInfo)
            {
                wordInfos.Add(info);
            }
            input.Init(wordInputs.Count,info);

            row.SetValueWithoutNotify(0);
            column.SetValueWithoutNotify(0);
            wordText.text = "";
            isHorizontalToggle.SetIsOnWithoutNotify(true);

            input.OnDelete += OnDeleteWord;
            if (wordInputs.Count == max)
            {
                newWordButton.interactable = false;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayout.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout2.transform as RectTransform);
        }
    }

    private void UpdateCanvas()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
    }

    public void SetEdit(NewWordInput input)
    {
        editingInput = input;
    }

    public void CancelEditing(NewWordInput input)
    {
        if (editingInput == input)
        {
            editingInput = null;
        }
    }

    public void OnClickCell(CellItem.Coord coords)
    {
        if (editingInput != null)
        {
            editingInput.SetCoords(coords);
        }
        else
        {
            SetCoords(coords);
        }
    }

    public void SetCoords(CellItem.Coord coord)
    {
        row.SetValueWithoutNotify( row.options.FindIndex(x => x.text == coord.row.ToString()));
        column.SetValueWithoutNotify( column.options.FindIndex(x => x.text == coord.column.ToString()));
    }
    
    private void OnDeleteWord(NewWordInput input, bool deleteInfo)
    {
        input.OnDelete -= OnDeleteWord;
        Destroy(input.gameObject);
        wordInputs.Remove(input);
        if(deleteInfo)
            wordInfos.Remove(input.Info);
        grid.ClearInterval(input.Info);
        for (int i = input.Index; i < wordsContainer.childCount; i++)
        {
            wordsContainer.GetChild(i).GetComponent<NewWordInput>().SetIndex(i);
            grid.UpdateWordIndex(wordsContainer.GetChild(i).GetComponent<NewWordInput>().Info.Coord, i);
        }
        newWordButton.interactable = true;
    }

    private void OnChangeTipType(bool isImage)
    {
        int max = maxItems[Utils.BoolToInt(isImage)];
        gridLayout.constraintCount = Utils.BoolToInt(isImage) + 1;
        if (this.isImage)
        {
            gridLayout.cellSize = new Vector2(400, 250);
        }
        else
        {
            gridLayout.cellSize = new Vector2(700, 200);
        }
        if (wordInputs.Count > max)
        {
            alertMessage.text = String.Format("Confirma alterar o tipo de dica e descartar {0} palavras já cadastradas?", wordInputs.Count -  max);
            confirmChangePanel.gameObject.SetActive(true);
        }
        else
        {
            ChangeTipType();
        }
    }

    private void ChangeTipType()
    {
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];
        
        for (int i = wordInputs.Count-1; i >= 0; i--)
        {
            OnDeleteWord(wordInputs[i], false);
        }
        
        for (int i = 0; i < wordInfos.Count; i++)
        {
            if (i < max)
            {
                CreateNewWord(wordInfos[i], false);
            }
            else
            {
                wordInfos.Remove(wordInfos[i]);
            }
        }
    }

    private void ResetToggle()
    {
        isImage.SetIsOnWithoutNotify(!isImage.isOn);
    }
    
    

}
