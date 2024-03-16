using System.Collections;
using System.Reflection;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.PropertyDrawer
{
	[CustomPropertyDrawer(typeof(StateMachineModel))]
	public class StateMachineModelPropertyDrawer : UnityEditor.PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			
			var propertyRect = new Rect(position.x, position.y, position.width - 55, position.height);
			var buttonRect = new Rect(position.x + position.width - 50, position.y, 50, position.height);
			
			EditorGUIUtility.labelWidth = 1;
			EditorGUI.PropertyField(propertyRect, property, label, true);
			EditorGUIUtility.labelWidth = 0;
			
			var instance = GetInstance<StateMachineModel>(property);
			if (instance != null && GUI.Button(buttonRect, "Open"))
			{
				var currentModel = ModelSelection.ActiveModel as StateMachineModel;
				instance.SetParent(currentModel);
				
				ModelSelection.ActiveModel = instance;
			}
			
			EditorGUI.EndProperty();
		}

		private static T GetInstance<T>(SerializedProperty property) where T : class
		{
			var index = GetListIndex(property.propertyPath);

			return index != -1 
				? GetInstanceFromList<T>(property, index) 
				: property.objectReferenceValue as T;
		}

		private static T GetInstanceFromList<T>(SerializedProperty property, int index) where T : class
		{
			// Extract the field name from the property path
			string fieldName = ExtractListFieldName(property.propertyPath);
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
			// Split the path into parts and find the part just before ".Array"
			var parts = path.Split('.');
			for (int i = 0; i < parts.Length - 1; i++)
			{
				if (parts[i + 1] == "Array")
					return parts[i];
			}
			return null;
		}
	}
}