using System.Collections.Generic;
using System.Linq;
using LubyLib.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class WordsGrid : MonoBehaviour
{
    public static char[] idsRow = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
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

            if (letter.ToString().ToUpperInvariant() == cellsDictionary[coord].Letter.ToString().ToUpperInvariant())
            {
                return true;
            }

            if(previousWord != null && !previousWord.IsNullEmptyOrWhitespace())
                return cellsDictionary[coord].CanReplace(previousWord);
            
        }

        return false;
    }

    public bool CheckInterval(NewWordInput.WordInfo info, string previousWord)
    {
        if (info.IsHorizontal)
        {
            for (int i = 0; i < info.Word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord(){
                        row = info.Coord.row,
                        column = info.Coord.column + i
                };
                if (!CheckCellAvailability(currentCoord, info.Word[i], previousWord))
                {
                    return false;
                }
            }
        }
        else
        {
            int j = idsRow.ToList().FindIndex(x => x == info.Coord.row);
            for (int i = 0; i < info.Word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord()
                {
                    row = idsRow[j+i],
                    column = info.Coord.column
                };
                if (!CheckCellAvailability(currentCoord, info.Word[i],previousWord))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CheckIfFitOnGrid(NewWordInput.WordInfo info)
    {
        CellItem.Coord lastCoord = new CellItem.Coord();
        if (info.IsHorizontal)
        {
            lastCoord = new CellItem.Coord()
            {
                row = info.Coord.row,
                column = info.Coord.column + info.Word.Length - 1
            };
            return cellsDictionary.ContainsKey(lastCoord);
        }
        
        int j = idsRow.ToList().FindIndex(x => x == info.Coord.row);
       
        if (j + info.Word.Length - 1 > idsRow.Length-1) return false;
       
        return true;
    }

    public void FillInterval(NewWordInput.WordInfo info, int index)
    {
        if (info.IsHorizontal)
        {
            for (int i = 0; i < info.Word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord(){
                    row = info.Coord.row,
                    column =info.Coord.column + i
                };
                
                cellsDictionary[currentCoord].FillCell(info.Word[i], info.Word, i == 0 ? index : -1);
            }
        }
        else
        {
            int j = idsRow.ToList().FindIndex(x => x == info.Coord.row);
            for (int i = 0; i < info.Word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord()
                {
                    row = idsRow[j+i],
                    column = info.Coord.column
                };

                cellsDictionary[currentCoord].FillCell(info.Word[i], info.Word,  i == 0 ? index : -1);
            }
        }
    }
    
    public void ClearInterval(NewWordInput.WordInfo info)
    {
        if (info.IsHorizontal)
        {
            for (int i = 0; i < info.Word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord(){
                    row = info.Coord.row,
                    column =info.Coord.column + i
                };
                cellsDictionary[currentCoord].ClearCell(info.Word);
            }
        }
        else
        {
            int j = idsRow.ToList().FindIndex(x => x == info.Coord.row);
            for (int i = 0; i < info.Word.Length; i++)
            {
                CellItem.Coord currentCoord = new CellItem.Coord()
                {
                    row = idsRow[j+i],
                    column = info.Coord.column
                };
                cellsDictionary[currentCoord].ClearCell(info.Word);
            }
        }
    }

    public void UpdateWordIndex(CellItem.Coord cell, int number)
    {
        cellsDictionary[cell].UpdateId(number);
    }

}
