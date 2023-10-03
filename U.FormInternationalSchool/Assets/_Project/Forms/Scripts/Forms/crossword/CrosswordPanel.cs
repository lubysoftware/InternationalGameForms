using System;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using LubyLib.Core.Singletons;
using TMPro;
using UnityEngine;
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

    [Space(15)] [Header("Warning panel")] [SerializeField]
    private Transform confirmChangePanel;

    [SerializeField] private Button confirmButton;
    [SerializeField] private Button denyButton;
    [SerializeField] private TextMeshProUGUI alertMessage;

    [Space(15)] [Header("New word fields")] [SerializeField]
    private TMP_Dropdown row;

    [SerializeField] private TMP_Dropdown column;
    [SerializeField] private TMP_InputField wordText;
    [SerializeField] private Toggle isHorizontalToggle;

    [Space(15)] [Header("Layouts")] [SerializeField]
    private LayoutGroup layout;

    [SerializeField] private LayoutGroup layout2;
    [SerializeField] private GridLayoutGroup gridLayout;

    private List<NewWordInput> wordInputs;
    private Dictionary<int, NewWordInput.WordInfo> wordInfos;

    public NewWordInput editingInput = null;

    public bool IsImage => isImage.isOn;
    public int WordsQtt => wordInputs.Count;

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
        wordInfos = new Dictionary<int, NewWordInput.WordInfo>();
    }

    public bool CheckWord(NewWordInput.WordInfo info, string previousWord, bool addInfo)
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

        if (!grid.CheckInterval(info, previousWord))
        {
            SetError(CrosswordError.MATCH, null);
            return false;
        }

        if (!previousWord.IsNullEmptyOrWhitespace())
        {
            NewWordInput previousInput = wordInputs.Find(x => info.index == x.Index);
            grid.ClearInterval(previousInput.Info);
            wordInfos[previousInput.Info.index] = info;
        }
        else
        {
            if (addInfo)
                wordInfos.Add(info.index, info);
        }

        grid.FillInterval(info, info.index);

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
                    IsHorizontal = isHorizontalToggle.isOn,
                    index = wordInputs.Count + 1
                };
                CreateNewWord(info, true);
            }

        }
    }

    private NewWordInput CreateNewWord(NewWordInput.WordInfo info, bool addInfo)
    {
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];
        if (CheckWord(info, null, addInfo))
        {
            NewWordInput input = Instantiate(wordInput[Utils.BoolToInt(isImage.isOn)], wordsContainer);
            wordInputs.Add(input);
            input.Init(wordInputs.Count, info);

            row.SetValueWithoutNotify(0);
            column.SetValueWithoutNotify(0);
            wordText.text = "";
            isHorizontalToggle.SetIsOnWithoutNotify(true);

            input.OnDelete += OnDeleteWord;
            if (wordInputs.Count == max)
            {
                newWordButton.interactable = false;
            }

            UpdateCanvas();
            form.GetComponent<CrosswordForm>().UpdatePoints();
            return input;
        }

        return null;
    }

    private void UpdateCanvas()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayout.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(layout2.transform as RectTransform);
    }

    public void SetEdit(NewWordInput input)
    {
        editingInput = input;
        foreach (NewWordInput inp in wordInputs)
        {
            if (inp != input)
            {
                inp.EditStatus(false);
            }
        }

        SetNewWordInteractable(false);
    }

    public void CancelEditing(NewWordInput input)
    {
        foreach (NewWordInput inp in wordInputs)
        {
            inp.EditStatus(true);
        }

        SetNewWordInteractable(true);
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
        row.SetValueWithoutNotify(row.options.FindIndex(x => x.text == coord.row.ToString()));
        column.SetValueWithoutNotify(column.options.FindIndex(x => x.text == coord.column.ToString()));
    }

    private void OnDeleteWord(NewWordInput input, bool deleteInfo)
    {
        input.OnDelete -= OnDeleteWord;
        if (deleteInfo)
            wordInfos.Remove(input.Index);
        for (int i = input.Index; i < wordInputs.Count; i++)
        {
            // NewWordInput inp = wordsContainer.GetChild(i).GetComponent<NewWordInput>();
            NewWordInput inp = wordInputs[i];
            wordInfos.Remove(inp.Index);
            inp.Info.index = i;
            wordInfos.Add(i, inp.Info);
            inp.SetIndex(i);
            grid.UpdateWordIndex(inp.Info.Coord, i);
        }

        Destroy(input.gameObject);
        wordInputs.Remove(input);
        grid.ClearInterval(input.Info);
        newWordButton.interactable = true;
        form.GetComponent<CrosswordForm>().UpdatePoints();
        Invoke(nameof(UpdateCanvas), 0.1f);
    }

    private void OnChangeTipType(bool isImage)
    {
        int max = maxItems[Utils.BoolToInt(isImage)];
        gridLayout.constraintCount = Utils.BoolToInt(isImage) + 1;

        if (wordInputs.Count > max)
        {
            int qtt = wordInputs.Count - max;
            string word = "palavras";
            if (qtt == 1)
            {
                word = "palavra";
            }

            alertMessage.text = String.Format("Confirma alterar o tipo de dica e descartar {0} {1} já cadastradas?",
                wordInputs.Count - max, word);
            confirmChangePanel.gameObject.SetActive(true);
        }
        else
        {
            ChangeTipType();
        }

    }

    public void CancelChangeTipType()
    {
        isImage.isOn = !isImage.isOn;
    }

    private void ChangeTipType()
    {
        if (isImage)
        {
            gridLayout.cellSize = new Vector2(400, 250);
        }
        else
        {
            gridLayout.cellSize = new Vector2(700, 200);
        }
        int max = maxItems[Utils.BoolToInt(isImage.isOn)];

        for (int i = wordInputs.Count - 1; i >= 0; i--)
        {
            OnDeleteWord(wordInputs[i], false);
        }

        int count = wordInfos.Count;
        for (int i = 0; i < count; i++)
        {
            if (i < max)
            {
                CreateNewWord(wordInfos[i+1], false);
            }
            else
            {
               wordInfos.Remove(wordInfos[i+1].index); 
            }
        }
        Canvas.ForceUpdateCanvases();
    }

    private void ResetToggle()
    {
        isImage.SetIsOnWithoutNotify(!isImage.isOn);
    }

    public List<File> GetImages()
    {
        List<File> listImages = new List<File>();
        foreach (var input in wordInputs)
        {
            ImageElement el = input.imageComp.Image;
            if (el.IsActive && el.UploadedFile != null)
            {
                listImages.Add(el.UploadedFile);
            }
        }
        return listImages;
    }
    
    public Dictionary<NewWordInput.WordInfo,string> FilledImage()
    {
        Dictionary<NewWordInput.WordInfo, string> listFilledImages = new Dictionary<NewWordInput.WordInfo, string>();
        foreach (var input in wordInputs)
        {
            ImageElement el = input.imageComp.Image;
            if (el.IsActive && el.UploadedFile == null)
            {
                if (el.IsFilled)
                {
                    listFilledImages.Add(input.Info, el.url);
                }
            }
        }

        return listFilledImages;
    }

    public bool IsAllFilled()
    {
        foreach (var input in wordInputs)
        {
            if (!input.IsFilled)
            {
                return false;
            }
        }

        return true;
    }

    public Dictionary<NewWordInput.WordInfo, string> FilledInputs()
    {
        Dictionary<NewWordInput.WordInfo, string> listInputs = new Dictionary<NewWordInput.WordInfo, string>();
        foreach (var input in wordInputs)
        {
            listInputs.Add(input.Info, input.GetTipText());
        }

        return listInputs;
    }
    

    public void FillData(List<WordsGet> words, Action<UploadFileElement, string, string> action, bool isImage)
    {
        for(int i = 0; i< words.Count; i++)
        {
            NewWordInput.WordInfo info = new NewWordInput.WordInfo()
            {
                Coord = new CellItem.Coord(){row = WordsGrid.idsRow[words[i].posX], column = words[i].posY +1},
                Word = words[i].answer,
                IsHorizontal = words[i].orientation == "HORIZONTAL",
                index = i+1
            };
  
           NewWordInput input = CreateNewWord(info, true);
           if (input != null)
           {
               if (isImage)
               {
                   ImageElement el = input.imageComp.Image;
                   action.Invoke(el, "tip", words[i].question);
               }
               else
               {
                   input.SetTipText(words[i].question);
               }
           }
        }
    }

    public List<NewWordInput> GetWordInputs()
    {
        return wordInputs;
    }

    public void SetImageToggle(bool status)
    {
        isImage.isOn =status;
        if (status)
        {
            gridLayout.constraintCount = 2;
        }
        if (status)
        {
            gridLayout.cellSize = new Vector2(400, 250);
        }
        else
        {
            gridLayout.cellSize = new Vector2(700, 200);
        }
    }
    
    private void SetNewWordInteractable(bool status)
    {
        wordText.interactable = status;
        row.interactable = status;
        column.interactable = status;
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

        if (status)
        {
            newWordButton.interactable = wordInputs.Count < maxItems[Utils.BoolToInt(isImage.isOn)];
        }
        else
        {
            newWordButton.interactable = false;
        }
    }
    

}
