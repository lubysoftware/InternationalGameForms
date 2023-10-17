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
   [SerializeField] private Canvas canvas;
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
      if (!DragDropPanel.Instance.CanDropHere(Image.showImage.gameObject.GetComponent<RectTransform>(), transform.GetSiblingIndex()))
      {
         gameObject.transform.localPosition = previousPosition;
      }
   }

   public void OnDrag(PointerEventData eventData)
   {
      gameObject.GetComponent<RectTransform>().anchoredPosition += eventData.delta/canvas.scaleFactor;
   }

   public void OnBeginDrag(PointerEventData eventData)
   {
      previousPosition = GetComponent<RectTransform>().localPosition;
   }
   
   public bool CanDropHere(Vector3[] corners)
   {
      foreach (var pos in corners.ToList())
      {
         if (RectTransformUtility.RectangleContainsScreenPoint(Image.showImage.gameObject.GetComponent<RectTransform>(), pos))
         {
            Debug.LogError(this.name);
            return false;
         }
      }

      return true;
   }
}
