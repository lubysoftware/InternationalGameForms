using UnityEngine;

namespace LubyLib.Core.Extensions
{
	public static class RectTransformExtensions
	{
		/// <summary>
		///   <para>Sets the anchor of the RectTransform to conform to its parent size.</para>
		/// </summary>
		/// <param name="keepSize">Should the initial size and position be kept.</param>
		public static void SetAnchorsExpanded(this RectTransform rectTransform, bool keepSize = false)
		{
			if (!keepSize)
			{
				rectTransform.anchorMin = Vector2.zero;
				rectTransform.anchorMax = Vector2.one;
			
				rectTransform.offsetMin = Vector2.zero;
				rectTransform.offsetMax = Vector2.zero;
				return;
			}

			Rect startRect = rectTransform.rect;
			var startPosition = rectTransform.position;
			
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;

			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startRect.width);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startRect.height);
			rectTransform.position = startPosition;





		}
	}
}