using System;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class CounterWithTargetNodeView : BaseStateNodeView
	{
		private readonly StateMachineModel _model;
		private readonly Type _stateType;
		private VisualElement _titleContainer;
		private VisualElement _title;
		private GraphView _graphView;
		private VisualElement _glowBorder;
		private IntegerField _field;
		private readonly BaseCounterState _counterState;
		private readonly VisualElement _propertyContainer;

		public CounterWithTargetNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel,  
							 StateNodeModel nodeModel) 
							 : base(graphView, stateMachineModel, nodeModel)
		{
			
			_counterState = (BaseCounterState) nodeModel.State;
			var contents = this.Query<VisualElement>("contents").First();
			
			AddStyle(nameof(CounterWithTargetNodeView));
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel();
			AddTitleIcon();
			AddEditButton();
			AddProgressBar();
			AddInputPorts(inputContainer);
			AddOutputPorts(outputContainer);
			
			_propertyContainer = CreatePropertyContainer();
			_propertyContainer.AddToClassList("full-width");
			contents.Insert(0, _propertyContainer);
			
			AddProperties(_propertyContainer);
			AddCounterField();
			AddGlowBorder();
			CheckCustomWidth();
			UpdatePosition();
		}

		public override void Update()
		{
			UpdateGlowBorder();
			UpdateField();
		}

		private void UpdateField()
		{
			_field.value = _counterState.Count;
		}

		private void AddCounterField()
		{
			_field = new IntegerField("Count");
			_field.isReadOnly = true;
			_field.AddToClassList("count-field");

			var scroll = _propertyContainer.Q<ScrollView>();
			scroll.Insert(0, _field);
		}
	}
}