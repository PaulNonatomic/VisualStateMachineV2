using System.Collections;
using System.Reflection;
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
			EditorGUI.BeginProperty(position, label, property);
			
			var propertyRect = new Rect(position.x, position.y, position.width - 55, position.height);
			
			EditorGUIUtility.labelWidth = 1;
			EditorGUI.PropertyField(propertyRect, property, label, true);
			EditorGUIUtility.labelWidth = 0;

			DrawOpenButton(position, property);
			
			EditorGUI.EndProperty();
		}

		private void DrawOpenButton(Rect position, SerializedProperty property)
		{
			var buttonRect = new Rect(position.x + position.width - 50, position.y, 50, position.height);
			var instance = PropertyUtils.GetInstance<StateMachineModel>(property);
			if (instance == null || !GUI.Button(buttonRect, "Open")) return;
			
			var currentModel = ModelSelection.ActiveModel as StateMachineModel;
			instance.SetParent(currentModel);
				
			ModelSelection.ActiveModel = instance;
		}
	}
}