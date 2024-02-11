using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	[CustomEditor(typeof(StateMachineController))]
	public class StateMachineControllerEditor : UnityEditor.Editor
	{
		private Button _newModelButton;
		private Button _openModelButton;

		public override VisualElement CreateInspectorGUI()
		{
			var controller = (StateMachineController) target;
			var root = CreateRoot();
			var stateMachineField = CreateStateMachineField(root, controller);
			
			_newModelButton = CreateNewModelButton(root);
			_openModelButton = CreateOpenModelButton(root, controller);
			
			return root;
		}

		private Button CreateNewModelButton(VisualElement container)
		{
			var newButton = new Button(() => CreateNewStateMachine());
			newButton.text = "New";
			newButton.style.minWidth = 50;
			newButton.style.maxWidth = 100;
			
			container.Add(newButton);

			return newButton;
		}

		private Button CreateOpenModelButton(VisualElement container, StateMachineController controller)
		{
			var openButton = new Button(() => OpenStateMachine(controller));
			openButton.text = "Open";
			
			container.Add(openButton);

			return openButton;
		}

		private static VisualElement CreateRoot()
		{
			var root = new VisualElement();
			root.name = "root";
			root.style.flexDirection = FlexDirection.Row;

			return root;
		}

		private static VisualElement CreateContainer(VisualElement container)
		{
			var horizontalContainer = new VisualElement();
			horizontalContainer.name = "horizontal-container";
			container.Add(horizontalContainer);

			return horizontalContainer;
		}
		
		private void OpenStateMachine(StateMachineController controller)
		{
			NodeGraphEditorWindow.OpenWindow<StateGraphEditorWindow>(controller.Model);
		}
		
		private ObjectField CreateStateMachineField(VisualElement container, StateMachineController controller)
		{
			var stateMachineField = new ObjectField("State Machine")
			{
				objectType = typeof(StateMachineModel),
				allowSceneObjects = false,
				bindingPath = nameof(controller.Model),
				value = controller.Model
			}; 
			stateMachineField.style.flexGrow = 1;
			stateMachineField.style.flexShrink = 1;
			stateMachineField.RegisterValueChangedCallback(HandleStateMachineFieldValueChange);
			container.Add(stateMachineField);
			
			BindStateMachineField(stateMachineField, controller);

			return stateMachineField;
		}

		private void HandleStateMachineFieldValueChange(ChangeEvent<Object> evt)
		{
			_newModelButton.style.display = evt.newValue == null ? DisplayStyle.Flex : DisplayStyle.None;
			_openModelButton.style.display = evt.newValue == null ? DisplayStyle.None : DisplayStyle.Flex;
		}

		private static void BindStateMachineField(IBindable stateMachineField, Object controller)
		{
			var serializedController = new SerializedObject(controller);
			var modelProperty = serializedController.FindProperty("_model");
			stateMachineField.BindProperty(modelProperty);
		}

		private void CreateNewStateMachine()
		{
			var controller = (StateMachineController) target;
			controller.Model = ScriptableObjectUtils.CreateInstanceInProject<StateMachineModel>(selectInstance: false);
		}
	}
}