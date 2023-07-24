using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportMaterialCreation : MonoBehaviour
{
    [SerializeField] private MaterialInputARea inputAreaPrefab;
    [SerializeField] private Button newInputArea;
    [SerializeField] private Transform content;
    [SerializeField] private Button close;

    private List<MaterialInputARea> materialInputs;
    private List<Material> materials;
    void Start()
    {
        close.onClick.AddListener(CloseButton);
        newInputArea.onClick.AddListener(AddInputArea);
        materialInputs = new List<MaterialInputARea>();
        materials = new List<Material>();
        AddInputArea();
    }

    private void AddInputArea()
    {
        MaterialInputARea newInput = Instantiate(inputAreaPrefab, content);
        materialInputs.Add(newInput);
        newInput.OnDestroy += RemoveFromList;
        newInputArea.transform.SetAsLastSibling();
    }
    
    
    private void SaveMaterial()
    {
        if (materials.Count > 0)
        {
            materials.Clear();
        }
        for (int i = 0; i < materialInputs.Count; i++)
        {
            materials.Add(new Material()
            {
                index = i,
                isText = materialInputs[i].IsText,
                text = materialInputs[i].Text
            });
        }
    }

    private void CloseButton()
    {
        SaveMaterial();
        this.gameObject.SetActive(false);
    }

    private void RemoveFromList(MaterialInputARea area)
    {
        area.OnDestroy -= RemoveFromList;
        materialInputs.Remove(area);
    }
    
    
}

public struct Material
{
    public int index;
    public string text;
    public bool isText;
}