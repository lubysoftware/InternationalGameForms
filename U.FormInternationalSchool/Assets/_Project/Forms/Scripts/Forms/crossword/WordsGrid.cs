using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WordsGrid : MonoBehaviour
{
    public char[] idsRow = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
    [SerializeField] private int cellsQtt = 100;
    [SerializeField] private CellItem cell;
    [SerializeField] private LayoutGroup layoutGroup;
    private Dictionary<CellItem.Coord, CellItem> cellsDictionary;

    void Start()
    {
        cellsDictionary = new Dictionary<CellItem.Coord, CellItem>();
        for (int i = 0; i < cellsQtt / idsRow.Length; i++)
        {
            for (int j = 0; j < idsRow.Length; j++)
            {
                CellItem newCell = Instantiate(cell, this.transform);
                CellItem.Coord newCoord = new CellItem.Coord()
                {
                    row = idsRow[i],
                    column = j+1
                };
                newCell.SetIndex(newCoord);
                cellsDictionary.Add(newCoord, newCell);
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.transform as RectTransform);
    }

    public bool CheckCellAvailability(CellItem.Coord coord, char letter, string previousWord)
    {
        if (cellsDictionary.ContainsKey(coord))
        {
            if (!cellsDictionary[coord].IsFilled)
            {
                return true;
            }

            if (letter == cellsDictionary[coord].Letter)
            {
                return true;
            }
            else
            {
                return cellsDictionary[coord].CanReplace(previousWord);
            }
        }

        return false;
    }

    public bool CheckInterval(CellItem.Coord firstCoord, string word, bool isHorizontal, string previousWord)
    {
        if (isHorizontal)
        {
            for (int i = 0; i < word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord(){
                        row = firstCoord.row,
                        column = firstCoord.column + i
                };
                if (!CheckCellAvailability(currentCoord, word[i], previousWord))
                {
                    return false;
                }
            }
        }
        else
        {
            int j = idsRow.ToList().FindIndex(x => x == firstCoord.row);
            for (int i = 0; i < word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord()
                {
                    row = idsRow[j+i],
                    column = firstCoord.column
                };
                if (!CheckCellAvailability(currentCoord, word[i], word))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CheckIfFitOnGrid(CellItem.Coord firstCoord, int wordLength, bool isHorizontal)
    {
        CellItem.Coord lastCoord = new CellItem.Coord();
        if (isHorizontal)
        {
            lastCoord = new CellItem.Coord()
            {
                row = firstCoord.row,
                column = firstCoord.column + wordLength - 1
            };
            return cellsDictionary.ContainsKey(lastCoord);
        }
        
        int j = idsRow.ToList().FindIndex(x => x == firstCoord.row);
       
        if (j + wordLength - 1 > idsRow.Length) return false;
       
        return true;
    }

    public void FillInterval(CellItem.Coord firstCoord, string word, bool isHorizontal, int index)
    {
        if (isHorizontal)
        {
            for (int i = 0; i < word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord(){
                    row = firstCoord.row,
                    column = firstCoord.column + i
                };
                
                cellsDictionary[currentCoord].FillCell(word[i], word, i == 0 ? index : -1);
            }
        }
        else
        {
            int j = idsRow.ToList().FindIndex(x => x == firstCoord.row);
            for (int i = 0; i < word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord()
                {
                    row = idsRow[j+i],
                    column = firstCoord.column
                };

                cellsDictionary[currentCoord].FillCell(word[i], word,  i == 0 ? index : -1);
            }
        }
    }
    
    public void ClearInterval(CellItem.Coord firstCoord, string word, bool isHorizontal)
    {
        if (isHorizontal)
        {
            for (int i = 0; i < word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord(){
                    row = firstCoord.row,
                    column = firstCoord.column + i
                };
                cellsDictionary[currentCoord].ClearCell(word);
            }
        }
        else
        {
            int j = idsRow.ToList().FindIndex(x => x == firstCoord.row);
            for (int i = 0; i < word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord()
                {
                    row = idsRow[j+i],
                    column = firstCoord.column
                };
                cellsDictionary[currentCoord].ClearCell(word);
            }
        }
    }

    public void UpdateWordIndex(CellItem.Coord cell, int number)
    {
        cellsDictionary[cell].UpdateId(number);
    }

}
