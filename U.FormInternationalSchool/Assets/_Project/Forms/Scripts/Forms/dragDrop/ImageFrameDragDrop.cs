using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageFrameDragDrop : ImageFrame, IPointerEnterHandler, IPointerExitHandler, IEndDragHandler,IDragHandler, IBeginDragHandler
{
   [SerializeField] private TMP_Dropdown groupSelection;
   [SerializeField] private GameObject overlay;
   [SerializeField] private Vector3 previousPosition;
   [SerializeField] private RectTransform collisionRect;

   public RectTransform rect;
   protected override void Start()
   {
      updateImg.onClick.AddListener(OnUpdateButton);
      delete.onClick.AddListener(OnDeleteButton);
   }

   private void Awake()
   {
      rect = GetComponent<RectTransform>();
   }

   public override void SetActiveState(bool state)
   {
      isActive = state;
      if(newImageButton != null) 
         newImageButton.gameObject.SetActive(!state);
   }

   public bool IsCompleted()
   {
      if (!IsActive || (Image.UploadedFile == null && !Image.IsFilled))
         return false;
      
      return true;
   }

   public void Activate(bool status)
   {
      if (status)
      {
         this.gameObject.SetActive(true);
      }
      else
      {
         gameObject.SetActive(false);
         groupSelection.value = -1;
         OnDeleteButton();
      }
   }

   private void OnHover(bool status)
   {
      overlay.gameObject.SetActive(status);
      delete.gameObject.SetActive(status);
      updateImg.gameObject.SetActive(status);
   }

   public void OnPointerEnter(PointerEventData eventData)
   {
      if(isActive)
         OnHover(true);
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      OnHover(false);
   }
   
   public override void OnDeleteButton()
   {
      SetActiveState(false);
      OnHover(false);
      Image.Delete();
   }

   protected override void OnUpdateButton()
   {
      OnHover(false);
      Image.AddImage();
   }
   
   public void OnEndDrag(PointerEventData eventData)
   {
      if (!DragDropPanel.Instance.CanDropHere(collisionRect, transform.GetSiblingIndex()))
      {
         gameObject.transform.localPosition = previousPosition;
      }
      else
      {
         Debug.LogError(transform.localPosition);
      }
   }

   public void OnDrag(PointerEventData eventData)
   {
      gameObject.GetComponent<RectTransform>().anchoredPosition += eventData.delta/DragDropPanel.Instance.canvas.scaleFactor;
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      transform.SetAsLastSibling();
      previousPosition = GetComponent<RectTransform>().localPosition;
   }
   
   public bool CanDropHere(Vector3[] corners)
   {
      foreach (var pos in corners.ToList())
      {
         if (RectTransformUtility.RectangleContainsScreenPoint(collisionRect, pos))
         {
            Debug.LogError(this.name);
            return false;
         }
      }

      return true;
   }

   public void GroupOptionsStatus(bool status)
   {
      groupSelection.gameObject.SetActive(status);
   }

   public void SetGroup(string group)
   {
      groupSelection.value = GetDropdownIndex(group);
   }
   
   private int GetDropdownIndex(string group)
   {
      return groupSelection.options.FindIndex(x => x.text == group);
   }
}
