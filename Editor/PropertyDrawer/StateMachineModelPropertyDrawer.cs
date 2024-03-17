﻿using System.Collections;
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
			
			EditorGUIUtility.labelWidth = PropertyUtils.IsListElement(property)
				? 1
				: 100;
			
			EditorGUI.PropertyField(propertyRect, property, label, true);
			EditorGUIUtility.labelWidth = 0;

			DrawButton(position, property);
			
			EditorGUI.EndProperty();
		}

		private static void DrawButton(Rect position, SerializedProperty property)
		{
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

			switch (property.serializedObject.targetObject)
			{
				case StateMachineController:
					//don't set parent
					break;
				default:
					var currentModel = ModelSelection.ActiveModel as StateMachineModel;
					instance.SetParent(currentModel);
					break;
			}
			
			ModelSelection.ActiveModel = instance;
		}

		private static void DrawNewButton(Rect buttonRect, SerializedProperty property)
		{
			if (!GUI.Button(buttonRect, "New")) return;
			
			var model = ScriptableObjectUtils.CreateInstanceInProject<StateMachineModel>(selectInstance: false);
			property.objectReferenceValue = model;
		}
	}
}