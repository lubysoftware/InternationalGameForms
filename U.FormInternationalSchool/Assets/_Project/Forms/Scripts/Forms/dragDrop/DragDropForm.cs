using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragDropForm : FormScreen
{
    [SerializeField] private DragDropPanel panel;
    [SerializeField] private InputElement failsPenalty;

    private int imageQtt;
    private Dictionary<int, string> filledImages;

    private int failsPenaltyValue = 0;

    private int elementsQtt;


    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt = 1;
            FillUploadFiles(backgroundMusic, "music_theme",
                "https://stg1atividades.blob.core.windows.net/arquivos/c0c977d6-ba42-43ea-94c2-29793d77889d_name.004_dragndrop.ogg");
        }
    }

    public override void FinishDownloadingGame(string text)
    {
        if (text != null)
        {
            FillBaseData(JsonConvert.DeserializeObject<BaseGameJson>(text));
            FillGameData(JsonConvert.DeserializeObject<DragDropJsonGet>(text));
        }
    }


    protected override void SendGameFiles()
    {
        filledImages = panel.FilledImages();
        imageQtt = panel.previousDropdown;

        if (panel.GetImages() != null && panel.GetImages().Count > 0)
        {
            SendFilesToAPI.Instance.StartUploadFiles(this, panel.GetImages(), false);
        }
        else
        {
            SerializeGameData(filledImages.Values.ToArray());
        }

    }

    protected override void CheckEmptyGameFields()
    {
        if (failsPenalty.InputField.text.IsNullEmptyOrWhitespace())
        {
            failsPenalty.ActivateErrorMode();
            emptyField.Add("Pontuação descontada por erro");
        }
        else
        {
            DeactivateErrorInput(failsPenalty);
        }

        if (!panel.BackImageIsFilled())
        {
            emptyField.Add("Imagem de fundo do grid");
        }

        if (emptyField.Count > 0)
        {
            if (emptyField.Count == 1)
            {
                ShowError(emptyField[0], ErrorType.EMPTY, null);
                return;
            }

            ShowError("", ErrorType.ALL_FIELDS, null);
            return;
        }

        ValidateFields();
    }

    public void CallCheckFails()
    {
        CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro");
    }

    protected override void ValidateFields()
    {
        base.ValidateFields();
        if (hasValidationError)
        {
            return;
        }

        if (CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro"))
        {
            int.TryParse(failsPenalty.InputField.text, out failsPenaltyValue);
        }
        else
        {
            return;
        }

        if (!panel.AllImagesFilled())
        {
            ShowError("Todos os elementos devem ser preenchidos.", ErrorType.CUSTOM, null);
            return;
        }

        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game" + urls);

        /*List<DraggableItem> listSeq = new List<DraggableItem>();
        if (filledImages.Count == imageQtt)
        {
            if (urls != null)
            {
                for (int i = 0; i < urls.Length; i++)
                {
                    listSeq.Add(new DraggableItem() { spawnPointX = i, imageUrl = urls[i] });
                }
            }
        }
        else
        {
            int urlIndex = 0;
            for (int i = 0; i < imageQtt; i++)
            {
                if (filledImages.ContainsKey(i))
                {
                    listSeq.Add(new DraggableItem() { position = i, imageUrl = filledImages[i] });
                }
                else
                {
                    if (urls.Length > urlIndex)
                    {
                        listSeq.Add(new DraggableItem() { position = i, imageUrl = urls[urlIndex] });
                        urlIndex++;
                    }
                }
            }
        }*/


       /* FormDragDrop completeForm = new FormDragDrop()
        {
            game = this.game,
            gameData = new DragDrop()
            {
                dropPlaceBackgroundUrl = "url",
                draggableItems = new List<DraggableItem>(){
            }
        };


        string json = JsonConvert.SerializeObject(completeForm);
        if (isEdit)
        {
            SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.InputField.text, this);
        }
        else
        {
            SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this);
        }*/
    }


    private void FillGameData(DragDropJsonGet json)
    {
        //failsPenalty.InputField.text = json.failPenalty.ToString();
        FillUploadFiles(panel.BackImage(), "grid_image", json.dropPlaceBackgroundUrl);
        panel.previousDropdown = json.draggableItem.Count;
        panel.FillToggles(false,json.draggableItem[0].dragType == "UNIQUE");
        panel.InstantiateFilledElements(json.draggableItem);
        elementsQtt = json.draggableItem.Count;
        loadFileQtt = loadFileQtt + elementsQtt;
        CheckIfMaxQtt();
    }

   

}

[Serializable]
public class DragDrop
{
    public string dropPlaceBackgroundUrl;
    public List<DraggableItem> draggableItems;
}


[Serializable]
public class FormDragDrop
{
    public FormBase game;
    public DragDrop gameData;
}

[Serializable]
public struct DraggableItem
{
    public string dragType;
    public string imageUrl;
    public float spawnPointX;
    public float spawnPointY;
}
