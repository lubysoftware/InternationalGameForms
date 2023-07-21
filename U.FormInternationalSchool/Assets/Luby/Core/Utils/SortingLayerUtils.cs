using UnityEngine;

namespace LubyLib.Core
{
	public static class SortingLayerUtils
	{
		public static string[] GetSortingLayerNames()
		{
			int layerCount = SortingLayer.layers.Length;

			string[] layerNames = new string[layerCount];

			for (int i = 0; i < layerCount; i++)
			{
				layerNames[i] = SortingLayer.layers[i].name;
			}

			return layerNames;
		}

		public static int GetSortingLayerIndexFromValue(int value)
		{
			int layerCount = SortingLayer.layers.Length;

			for (int i = 0; i < layerCount; i++)
			{
				if (value == SortingLayer.layers[i].value)
					return i;
			}

			return -1;
		}

		public static int GetSortingLayerIndexFromSortingLayerID(int id)
		{
			int layerCount = SortingLayer.layers.Length;

			for (int i = 0; i < layerCount; i++)
			{
				if (id == SortingLayer.layers[i].id)
					return i;
			}

			return -1;
		}
		
		public static SortingLayer GetSortingLayerFromSortingLayerID(int id)
		{
			int layerCount = SortingLayer.layers.Length;

			for (int i = 0; i < layerCount; i++)
			{
				var sortingLayer = SortingLayer.layers[i];
				if (id == sortingLayer.id)
					return sortingLayer;
			}

			return default;
		}
		
		

		public static SortingLayer[] GetSortingLayers() => SortingLayer.layers;

		public static SortingLayer GetSortingLayerFromIndex(int index) => GetSortingLayers()[index];
	}
}