using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core;
using LubyLib.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragDropForm : FormScreen
{
    [SerializeField] private DragDropPanel panel;
    [SerializeField] private InputElement failsPenalty;

    private int imageQtt;
    private Dictionary<int, string> filledImages;

    private int failsPenaltyValue = 0;

    private int elementsQtt;

    private List<DraggableItem> items; 
    private int uploadedFilesElementsQtt = 0;
    
    private string backImagePath = "";

    protected override void Start()
    {
        base.Start();
        isEdit = false;
        SceneDataCarrier.GetData(Constants.IS_EDIT, out isEdit);
        if (!isEdit)
        {
            loadFileQtt = 1;
            FillUploadFiles(backgroundMusic, "music_theme",
                themeSongsUrls[GameType.DRAGNDROP]);
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
        items = panel.GetAllDraggableItems();
        List<File> filesToSend = new List<File>();

        if (panel.BackImage().UploadedFile != null)
        {
            filesToSend.Add(panel.BackImage().UploadedFile);
            backImagePath = "";
        }
        else
        {
            if (panel.BackImage().IsFilled)
            {
                backImagePath = panel.BackImage().url;
            }
            else
            {
                if (isPreview)
                {
                    ShowError("Imagem de fundo do grid", ErrorType.EMPTY, null);
                    return;
                }else
                {
                    backImagePath = null;
                }
            }
        }
        
        
        if (items != null && items.Count > 0)
        {
            foreach (DraggableItem item in items)
            {
                if (item.image != null)
                {
                    filesToSend.Add(item.image);
                    uploadedFilesElementsQtt++;
                }
            }
        }
        
        
        if (filesToSend.Count > 0)
        {
            SendFilesToAPI.Instance.StartUploadFiles(this, filesToSend, false);
        }
        else
        {
            SerializeGameData(null);
        }

    }

    protected override void CheckEmptyGameFields()
    {
        if (failsPenalty.InputField.text.IsNullEmptyOrWhitespace())
        {
            failsPenalty.ActivateNullMode();
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
            if (isPreview)
            {
                if (emptyField.Count == 1)
                {
                    ShowError(emptyField[0], ErrorType.EMPTY, null);
                    return;
                }

                ShowError("", ErrorType.ALL_FIELDS, null);
                return;
            }

            isCompleted = false;
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

        if (!failsPenalty.Null)
        {
            if (CheckGreatherThanZero(failsPenalty, "Pontuação descontada por erro"))
            {
                int.TryParse(failsPenalty.InputField.text, out failsPenaltyValue);
            }
            else
            {
                return;
            }
            
        }else
        {
            isCompleted = false;
        }

        if (isPreview)
        {
            if (!panel.CheckIfAllElementsAreFullyComplete())
            {
                string errormessage = "Todos os elementos devem ser preenchidos.";
                if (panel.IsGroup)
                {
                    errormessage = "Todos os elementos devem ter imagem e número do grupo preenchidos.";
                }

                ShowError(errormessage, ErrorType.CUSTOM, null);
                return;
            }
        }

        SendBaseFormFiles();
    }

    public override void SerializeGameData(string[] urls)
    {
        Debug.Log("serialize game" + urls);

        if (urls != null)
        {
            previewUrlsToDelete.AddRange(urls.ToList());
        }

        int urlIndex = 0;
        if (backImagePath == "")
        {
            backImagePath = urls[0];
            urlIndex = 1;
        }

        List<DraggableItemJson> listDraggableItems = new List<DraggableItemJson>();
        if (urls == null)
        {
            for (int i = 0; i < filledImages.Count; i++)
            {
                listDraggableItems.Add(new DraggableItemJson()
                {
                    spawnPointX = items[i].spawnPointX, spawnPointY = items[i].spawnPointY, imageUrl = filledImages[i],
                    groupId = items[i].groupId
                });
            }
        }
        else
        {
            for (int i = 0; i < imageQtt; i++)
            {
                if (filledImages.ContainsKey(i))
                {
                    listDraggableItems.Add(new DraggableItemJson()
                    {
                        spawnPointX = items[i].spawnPointX, spawnPointY = items[i].spawnPointY,
                        imageUrl = filledImages[i], groupId = items[i].groupId
                    });
                }
                else
                {
                    if (urls.Length > urlIndex)
                    {
                        listDraggableItems.Add(new DraggableItemJson()
                        {
                            spawnPointX = items[i].spawnPointX, spawnPointY = items[i].spawnPointY,
                            imageUrl = urls[urlIndex], groupId = items[i].groupId
                        });
                        urlIndex++;
                    }
                }
            }
        }
        FormDragDrop completeForm = new FormDragDrop()
        {
            game = this.game,
            gameData = new DragDrop()
            {
                dropPlaceBackgroundUrl = backImagePath,
                failPenalty = failsPenaltyValue == 0 ? null : failsPenaltyValue,
                materialType = panel.IsText ? "TEXT" : "IMAGE",
                dragType = panel.IsGroup ? "CATEGORY" : "UNIQUE",
                draggableItems = listDraggableItems,
            }
        };

       
        if (!isPreview)
        {
            string json = JsonConvert.SerializeObject(completeForm);
            if (CheckIfHasImageToFill(json))
            {
                completeForm.game.isDraft = true;
                json = JsonConvert.SerializeObject(completeForm);
            }
            
            if (isEdit)
            {
                SendFilesToAPI.Instance.StartUploadJsonUpdate(json, so.url, id, title.InputField.text, this, SendGameInfoToPortal);
            }
            else
            {
                SendFilesToAPI.Instance.StartUploadJson(json, so.url, title.InputField.text, this, SendGameInfoToPortal);
            }
        }
        else
        {
            FormDragDropPreviewData preview = new FormDragDropPreviewData()
            {
                gameTitle = game.gameTitle,
                backgroundMusicUrl = game.backgroundMusicUrl,
                backgroundUrl = game.backgroundUrl,
                bonustimer = game.bonustimer,
                gameTitleImageUrl = game.gameTitleImageUrl,
                hasSupportMaterial = game.hasSupportMaterial,
                SupportMaterial = game.supportMaterial,
                hasTimer = game.hasTimer,
                questionStatementEnglishAudioUrl = game.questionStatementEnglishAudioUrl,
                questionStatementEnglishVersion = game.questionStatementEnglishVersion,
                questionStatementPortugueseAudioUrl = game.questionStatementPortugueseAudioUrl,
                timer = timeInSec,
                questionStatementPortugueseVersion = game.questionStatementPortugueseVersion,
                dropPlaceBackgroundUrl = completeForm.gameData.dropPlaceBackgroundUrl,
                failPenalty = completeForm.gameData.failPenalty,
                materialType = completeForm.gameData.materialType,
                dragType = completeForm.gameData.dragType,
                draggableItem = completeForm.gameData.draggableItems,
            };

            DragDropPreview previewData = new DragDropPreview()
            {
                previewData = preview,
                filesToDelete = previewUrlsToDelete
            };
            
            string json = JsonConvert.SerializeObject(previewData);
            
            PreviewInPortal(json);
        }
    }


    private void FillGameData(DragDropJsonGet json)
    {
        failsPenalty.InputField.text = json.failPenalty.ToString();
        CheckFillFile(panel.BackImage(), "grid_image", json.dropPlaceBackgroundUrl);
        panel.previousDropdown = json.draggableItem.Count;
        panel.FillToggles(json.materialType == "TEXT",json.dragType == "CATEGORY");
        panel.InstantiateFilledElements(json.draggableItem, OnLoadFile);
        elementsQtt = json.draggableItem.Count;
        loadFileQtt = loadFileQtt + elementsQtt;
        currentLoad += elementsQtt;
        CheckIfMaxQtt();
    }

    protected override void SetFailsPenalty(int points)
    {
        failsPenalty.InputField.text = points.ToString();
    }
   

}

[Serializable]
public class DragDrop
{
    public string dropPlaceBackgroundUrl;
    public Nullable<int> failPenalty;
    public string materialType;
    public string dragType;    
    public List<DraggableItemJson> draggableItems;
}


[Serializable]
public class FormDragDrop
{
    public FormBase game;
    public DragDrop gameData;
}

[Serializable]
public class FormDragDropPreviewData : BaseGameJson
{
    public string dropPlaceBackgroundUrl;
    public Nullable<int> failPenalty;
    public string materialType;
    public string dragType;    
    public List<DraggableItemJson> draggableItem;
}

[Serializable]
public class DragDropPreview
{
    public FormDragDropPreviewData previewData;
    public List<string> filesToDelete;
}

[Serializable]
public struct DraggableItem
{
    public int groupId;
    public float spawnPointX;
    public float spawnPointY;
    public File image;
}

[Serializable]
public struct DraggableItemJson
{
    public int groupId;
    public float spawnPointX;
    public float spawnPointY;
    public string imageUrl;
}

