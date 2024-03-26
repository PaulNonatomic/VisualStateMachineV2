using Nonatomic.VSM2.Blackboard;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.Blackboard
{
	public class BlackboardView : VisualElement
	{
		private TextField _keyField;
		private TextField _valueField;
		private Button _addButton;
		private Button _removeButton;
		private StateMachineModel _model;

		public BlackboardView()
		{
			CreateUI();
		}

		public void Populate(StateMachineModel model)
		{
			_model = model;
		}

		private void CreateUI()
		{
			_keyField = new TextField("Key");
			_valueField = new TextField("Value");

			_addButton = new Button(() => AddValue())
			{
				text = "Add"
			};

			_removeButton = new Button(() => RemoveValue())
			{
				text = "Remove"
			};

			Add(_keyField);
			Add(_valueField);
			Add(_addButton);
			Add(_removeButton);
		}

		private void AddValue()
		{
			var key = ScriptableObject.CreateInstance<VariableKey>();
			key.name = "New Variable";
			
			_model.Blackboard.TryAddValue(key, _valueField.value);
		}

		private void RemoveValue()
		{
			
		}
	}
}