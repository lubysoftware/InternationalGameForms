using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Dynamic;
using LubyLib.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Color disabledColor;

    [SerializeField] private Color activatedColor;

    [SerializeField] private Color emptyTextBackgroundColor;

    [SerializeField] private Image background;

    [SerializeField] private TextMeshProUGUI id;

    public Coord Coords;
    public bool IsFilled;
    public char Letter;

    private List<string> words;

    public struct Coord
    {
        public char row;
        public int column;
    }

    private void Start()
    {
        words = new List<string>();
        GetComponent<Button>().onClick.AddListener(() => CrosswordPanel.Instance.OnClickCell(Coords));
    }

    public void SetIndex(Coord coords)
    {
        Coords = coords;
        text.text = coords.row + coords.column.ToString();
        text.color = disabledColor;
        background.color = Color.white;
        IsFilled = false;
        Letter = '*';
        id.text = "";
    }

    public void FillCell(char letter, string word, int number)
    {
        Letter = letter;
        if (id.text.IsNullEmptyOrWhitespace())
        {
            UpdateId(number);
        }
        text.text = letter.ToString().ToUpperInvariant();
        text.color = activatedColor;
        if (letter.ToString().IsNullEmptyOrWhitespace())
        {
            background.color = emptyTextBackgroundColor;
        }
        else
        {
            background.color = Color.white;
        }
        IsFilled = true;
        words.Add(word);
    }

    public void ClearCell(string word)
    {
        if (words.Count == 1)
        {
            SetIndex(Coords);
        }

        words.Remove(word);
    }

    public bool CanReplace(string word)
    {
        return words.Count == 1 && word == words[0];
    }

    public void UpdateId(int number)
    {
        if (number > 0)
        {
            id.text = number.ToString();
        }
        else
        {
            id.text = "";
        }
    }

}
