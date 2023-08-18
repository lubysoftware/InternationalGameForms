using UnityEngine;
using UnityEngine.EventSystems;

public class DropSLot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.LogError("on drop");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(0, 0);
        }
    }
}
