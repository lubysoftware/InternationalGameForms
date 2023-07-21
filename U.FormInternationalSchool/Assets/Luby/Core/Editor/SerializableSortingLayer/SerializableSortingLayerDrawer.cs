using UnityEditor;
using UnityEngine;

namespace LubyLib.Core.Editor.SerializableSortingLayer
{
	[CustomPropertyDrawer(typeof(Core.SerializableSortingLayer.SerializableSortingLayer))]
	public class SerializableSortingLayerDrawer : PropertyDrawer
	{
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var idProperty = property.FindPropertyRelative("id");
			var oldId = idProperty.intValue;
			var oldIndex = SortingLayerUtils.GetSortingLayerIndexFromSortingLayerID(oldId);
			
			EditorGUI.BeginProperty(position, label, property);
			
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			int newIndex = EditorGUI.Popup(position, oldIndex, SortingLayerUtils.GetSortingLayerNames());

			if (newIndex != oldIndex)
			{
				var sortingLayer = SortingLayerUtils.GetSortingLayerFromIndex(newIndex);
				idProperty.intValue = sortingLayer.id;
			}

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}