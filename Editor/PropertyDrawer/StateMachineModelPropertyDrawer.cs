using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.StateGraph;
using Nonatomic.VSM2.Editor.Utils;
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
			if (GuardAgainstDestroyedSerializedObject(property)) return;
			
			EditorGUI.BeginProperty(position, label, property);
			
			var propertyRect = new Rect(position.x, position.y, position.width - 55, position.height);
			
			EditorGUIUtility.labelWidth = PropertyUtils.IsListElement(property) ? 1 : 100;
			EditorGUI.PropertyField(propertyRect, property, label, true);
			EditorGUIUtility.labelWidth = 0;

			DrawButton(position, property);
			
			EditorGUI.EndProperty();
		}

		private static void DrawButton(Rect position, SerializedProperty property)
		{
			if (GuardAgainstDestroyedSerializedObject(property)) return;
			
			var buttonRect = new Rect(position.x + position.width - 50, position.y, 50, position.height);
			var instance = PropertyUtils.GetInstance<StateMachineModel>(property);
			
			if (instance == null)
			{
				DrawNewButton(buttonRect, property);
			}
			else
			{
				DrawOpenButton(buttonRect, property, instance);
			}
		}

		private static void DrawOpenButton(Rect buttonRect, SerializedProperty property, StateMachineModel instance)
		{
			if (!GUI.Button(buttonRect, "Open")) return;
			if (GuardAgainstDestroyedSerializedObject(property)) return;

			NodeGraphEditorWindow.OpenWindow<StateGraphEditorWindow>();

			SetActiveModel(instance);
			SetModelParent(instance);
		}

		private static void SetActiveModel(StateMachineModel instance)
		{
			ModelSelection.ActiveModel = instance;
		}

		private static void SetModelParent(StateMachineModel instance)
		{
			var currentModel = ModelSelection.ActiveModel as StateMachineModel;
			if (!currentModel) return;
			
			instance?.SetParent(currentModel);
		}

		private static void DrawNewButton(Rect buttonRect, SerializedProperty property)
		{
			if (!GUI.Button(buttonRect, "New")) return;
			if (GuardAgainstDestroyedSerializedObject(property)) return;
			
			var model = ScriptableObjectUtils.CreateInstanceInProject<StateMachineModel>(selectInstance: false);
			if (model == null) return;
			
			property.objectReferenceValue = model;
		}
		
		private static bool GuardAgainstDestroyedSerializedObject(SerializedProperty property)
		{
			return property?.serializedObject == null || 
					property.serializedObject.targetObject == null;
		}
	}
}
