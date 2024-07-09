using Nonatomic.VSM2.StateGraph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateLabelView : VisualElement
	{
		private readonly string[] _stateLabels = {"Edit Mode", "View Mode", "Active Mode"};
		private readonly string[] _stateClasses = {"edit-mode", "play-mode", "active-mode"};
		private readonly Label _label;
		private StateMachineModel _model;

		public StateLabelView()
		{
			name = "state-label-view";
			
			var styleSheet = UnityEngine.Resources.Load<StyleSheet>(nameof(StateLabelView));
			styleSheets.Add(styleSheet);
			
			_label = new Label
			{
				name = "state-label-txt"
			};
			Add(_label);

			EditMode();
			
			schedule.Execute(Update).Every(100);
		}

		private void EditMode()
		{
			SetMode(0);
		}

		private void PlayMode()
		{
			SetMode(1);
		}

		private void ActiveMode()
		{
			SetMode(2);
		}
		
		private void SetMode(int index)
		{
			foreach(var className in _stateClasses)
			{
				this.RemoveFromClassList(className);
			}
			
			if(index < 0 || index >= _stateClasses.Length) return;
			
			AddToClassList(_stateClasses[index]);
			_label.text = _stateLabels[index];
		}

		public void SetModel(StateMachineModel model)
		{
			_model = model;
		}

		private void Update()
		{
			if(!_model) return;
			
			if (!Application.isPlaying)
			{
				EditMode();
			}
			else
			{
				if (!_model.Original || (_model.Original.name == _model.name))
				{
					PlayMode();
				}
				else
				{
					ActiveMode();
				}
			}
		}
	}
}