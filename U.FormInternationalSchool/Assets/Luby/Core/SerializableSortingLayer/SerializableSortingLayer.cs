using UnityEngine;

namespace LubyLib.Core.SerializableSortingLayer
{
	[System.Serializable]
	public class SerializableSortingLayer
	{
		public int id;
		public int value => SortingLayer.GetLayerValueFromID(id);
		public string name => SortingLayer.IDToName(id);
		public SortingLayer Layer => SortingLayerUtils.GetSortingLayerFromSortingLayerID(id);

		/*[CustomEditor(typeof(SerializableSortingLayer))]
		public class SortingLayerDrawer : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				
				EditorGUI.Popup()
			}
		}*/
	}
	
	
}