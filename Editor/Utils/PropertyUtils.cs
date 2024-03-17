using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class PropertyUtils
	{
		public static T GetInstance<T>(SerializedProperty property) where T : class
		{
			var index = GetListIndex(property.propertyPath);

			return index != -1 
				? GetInstanceFromList<T>(property, index) 
				: property.objectReferenceValue as T;
		}
		
		public static bool IsListElement(SerializedProperty property)
		{
			return GetListIndex(property.propertyPath) != -1;
		}

		private static T GetInstanceFromList<T>(SerializedProperty property, int index) where T : class
		{
			var fieldName = ExtractListFieldName(property.propertyPath);
			if (fieldName == null)
			{
				Debug.LogError("Could not find field name in property path: " + property.propertyPath);
				return default;
			}

			var list = GetFieldListValue(property.serializedObject.targetObject, fieldName);
			return list != null && index < list.Count 
				? (T)list[index] 
				: default;
		}

		private static int GetListIndex(string path)
		{
			var parts = path.Split('[');
			if (parts.Length <= 1) return -1; 
			
			var indexStr = parts[1].Split(']')[0];
			if (!int.TryParse(indexStr, out var index)) return -1;
			
			return index;
		}

		private static IList GetFieldListValue(object source, string fieldName)
		{
			var field = source.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			var list = field?.GetValue(source) as IList;
			return list;
		}
		
		private static string ExtractListFieldName(string path)
		{
			var parts = path.Split('.');
			for (var i = 0; i < parts.Length - 1; i++)
			{
				if (parts[i + 1] == "Array")
					return parts[i];
			}
			return null;
		}
	}
}