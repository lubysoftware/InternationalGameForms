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
    [SerializeField] private NewWordInput wordInput;
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
   
    private List<NewWordInput> wordInputs;

    private NewWordInput editingInput = null;

    public struct Word
    {
        public string word;
        public bool isHorizontal;
        public List<CellItem> cells;
    }
    
    public enum CrosswordError
    {
        FIT,
        MATCH, 
        BIG,
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(ChangeTipType);
        denyButton.onClick.AddListener(ResetToggle);
        newWordButton.onClick.AddListener(CreateNewWord);
       // isImage.onValueChanged.AddListener(OnChangeTipType);
       wordInputs = new List<NewWordInput>();
    }

    public bool CheckWord(CellItem.Coord coords, string word, bool isHorizontal, string previousWord, int index)
    {
        if (word.Length > 10)
        {
            SetError(CrosswordError.BIG, null);
            return false;
        }

        if (!grid.CheckIfFitOnGrid(coords, word.Length, isHorizontal))
        {
            SetError(CrosswordError.FIT, null);
            return false;
        }
        if(!grid.CheckInterval(coords, word, isHorizontal, previousWord))
        {
            SetError(CrosswordError.MATCH, null);
            return false;
        }

        if (!previousWord.IsNullEmptyOrWhitespace())
        {
            NewWordInput previousInput = wordInputs.Find(x => index == x.Index);
            grid.ClearInterval(previousInput.c,previousInput.word,previousInput.isHorizontal);
        }

        grid.FillInterval(coords, word, isHorizontal, index);
        
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
        }

        form.ShowError(error, ErrorType.CUSTOM, null);
    }
    

    private void CreateNewWord()
    {
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];
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
                if(CheckWord(coord,wordText.text,isHorizontalToggle.isOn, null, wordInputs.Count +1))
                {
                    NewWordInput input = Instantiate(wordInput, wordsContainer);
                    wordInputs.Add(input);
              
                    input.Init(wordInputs.Count,coord, wordText.text, isHorizontalToggle.isOn);

                    row.SetValueWithoutNotify(0);
                    column.SetValueWithoutNotify(0);
                    wordText.text = "";
                    isHorizontalToggle.SetIsOnWithoutNotify(true);

                    input.OnDelete += OnDeleteWord;
                    if (wordInputs.Count == max)
                    {
                        newWordButton.interactable = false;
                    }
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layout2.transform as RectTransform);
                }
            }
            
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
    
    private void OnDeleteWord(NewWordInput input)
    {
        input.OnDelete -= OnDeleteWord;
        Destroy(input.gameObject);
        wordInputs.Remove(input);
        grid.ClearInterval(input.c, input.word, input.isHorizontal);
        for (int i = input.Index; i < wordsContainer.childCount; i++)
        {
            wordsContainer.GetChild(i).GetComponent<NewWordInput>().SetIndex(i);
            grid.UpdateWordIndex(wordsContainer.GetChild(i).GetComponent<NewWordInput>().c, i);
        }
        newWordButton.interactable = true;
    }

    private void OnChangeTipType(bool isImage)
    {
        int max = maxItems[Utils.BoolToInt(isImage)];
        if (wordInputs.Count > max)
        {
            alertMessage.text = String.Format("Confirma alterar o tipo de dica e descartar {0} palavras já cadastradas?", wordInputs.Count -  max);
            confirmChangePanel.gameObject.SetActive(true);
        }
    }

    private void ChangeTipType()
    {
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];
        int extra = wordInputs.Count - max;

        if (extra > 0)
        {
            for (int i = 0; i < extra; i++)
            {
                OnDeleteWord(wordsContainer.GetChild(wordsContainer.childCount-1).GetComponent<NewWordInput>());
            }
        }

        for (int i = 0; i < wordInputs.Count; i++)
        {
            wordsContainer.GetChild(wordsContainer.childCount-1).GetComponent<NewWordInput>().ChangeType(isImage.isOn);
        }
    }

    private void ResetToggle()
    {
        isImage.SetIsOnWithoutNotify(!isImage.isOn);
    }
    
    

}
