using UnityEditor;
using UnityEngine;

namespace LubyLib.Core.Editor.ReadOnlyAttribute
{
    [CustomPropertyDrawer(typeof(Core.ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
	    [SerializeField]
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

}
