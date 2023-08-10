using System;
using System.Collections;
using System.Collections.Generic;
using FrostweepGames.Plugins.WebGLFileBrowser;
using LubyLib.Core.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class SupportMaterialCreation : MonoBehaviour
{
    [SerializeField] private MaterialInputArea inputAreaPrefab;
    [SerializeField] private Button newInputArea;
    [SerializeField] private Transform content;
    [SerializeField] private Button close;
    [SerializeField] private Transform blocker;

    private List<MaterialInputArea> materialInputs =  new List<MaterialInputArea>();
    private List<Material> materials = new List<Material>();
    void Start()
    {
        close.onClick.AddListener(CloseButton);
        newInputArea.onClick.AddListener(() => AddInputArea());
        materialInputs = new List<MaterialInputArea>();
        materials = new List<Material>();
        AddInputArea();
    }

    private MaterialInputArea AddInputArea()
    {
        MaterialInputArea newInput = Instantiate(inputAreaPrefab, content);
        materialInputs.Add(newInput);
        newInput.OnDestroy += RemoveFromList;
        newInputArea.transform.SetAsLastSibling();
        return newInput;
    }

    private void OnEnable()
    {
        blocker.gameObject.SetActive(true);
    }


    private void SaveMaterial()
    {
        if (materials.Count > 0)
        {
            materials.Clear();
        }
        for (int i = 0; i < materialInputs.Count; i++)
        {
            if (materialInputs[i].IsText && !materialInputs[i].Text.IsNullEmptyOrWhitespace())
            {
                materials.Add(new Material()
                {
                    index = i,
                    file = materialInputs[i].Image,
                    isText = materialInputs[i].IsText,
                    text = materialInputs[i].Text
                });
                
            }else if (!materialInputs[i].IsText && materialInputs[i].Image != null)
            {
                materials.Add(new Material()
                {
                    index = i,
                    file = materialInputs[i].Image,
                    isText = materialInputs[i].IsText,
                    text = materialInputs[i].Text
                });
            }
        }
    }

    private void CloseButton()
    {
        SaveMaterial();
        this.gameObject.SetActive(false);
    }

    private void RemoveFromList(MaterialInputArea area)
    {
        area.OnDestroy -= RemoveFromList;
        materialInputs.Remove(area);
    }

    public List<Material> GetSupportMaterial()
    {
        return materials;
    }

    public void FillSupportMaterial(List<SupportMaterialGet> materials)
    {
        foreach (SupportMaterialGet mat in materials)
        {
            MaterialInputArea area = AddInputArea();
            if (mat.materialType == "TEXT")
            {
                area.SetText(mat.material);
            }
            else
            {
                area.SetImage("support",mat.material);
            }
        }
    }
    
    
    
}

public struct Material
{
    public int index;
    public string text;
    public File file;
    public bool isText;
}

[Serializable]
public class SupportMaterial
{
    public int position;
    public string material;
    public string materialType;
}
