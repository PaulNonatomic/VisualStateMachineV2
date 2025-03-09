using System.Collections.Generic;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class NodePropertyPanel
	{
		private readonly BaseStateNodeView _nodeView;
		private readonly StateNodeModel _nodeModel;
	
		public NodePropertyPanel(BaseStateNodeView nodeView, StateNodeModel nodeModel)
		{
			_nodeView = nodeView;
			_nodeModel = nodeModel;
		}
	
		public VisualElement CreatePropertyContainer()
		{
			var propertyContainer = new VisualElement();
			propertyContainer.name = "property-container";
			return propertyContainer;
		}
	
		public void AddProperties(VisualElement container)
		{
			if (_nodeModel == null || _nodeModel.State == null) return;
		
			var scrollView = new ScrollView();
			container.Add(scrollView);

			var stateInspector = CreatePropertyInspector(_nodeModel.State);
			stateInspector.name = "state-inspector";
			scrollView.contentContainer.Add(stateInspector);
			
			if (stateInspector.childCount > 0)
			{
				container.AddToClassList("has-properties");
			}
		}
	
		public VisualElement CreatePropertyInspector(UnityEngine.Object target, 
			List<string> propertiesToExclude = null)
		{
			var container = new VisualElement();
			var serializedObject = new SerializedObject(target);
			var fields = FieldUtils.GetInheritedSerializedFields(target.GetType());

			foreach (var field in fields)
			{
				if (propertiesToExclude != null && propertiesToExclude.Contains(field.Name)) continue;

				var serializedProperty = serializedObject.FindProperty(field.Name);
				if (serializedProperty == null)
				{
					GraphLog.LogWarning($"Property {field.Name} not found in serialized object.");
					continue;
				}

				if (IsSubStateMachineList(serializedProperty))
				{
					// Handle SubStateMachine list
					var customElement = CreateCustomSubStateMachineUI(serializedProperty);
					container.Add(customElement);
				}
				else
				{
					// Handle regular properties
					var propertyField = new PropertyField(serializedProperty);
					container.Add(propertyField);
				}
			}

			container.Bind(serializedObject);
			return container;
		}
	
		private bool IsSubStateMachineList(SerializedProperty property)
		{
			if (!property.isArray) return false;
			return property.arrayElementType == "PPtr<$StateMachineModel>";
		}

		private VisualElement CreateCustomSubStateMachineUI(SerializedProperty property)
		{
			return new PropertyField(property);
		}
	}
}