using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class CounterNodeView : BaseStateNodeView
	{
		private readonly VisualElement _propertyContainer;
		private Image _icon;
		private IntegerField _field;
		private readonly BaseCounterState _counterState;

		public CounterNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel,  
							 StateNodeModel nodeModel) 
							 : base(graphView, stateMachineModel, nodeModel)
		{

			_counterState = (BaseCounterState) nodeModel.State;
			
			AddStyle(nameof(CounterNodeView));
			AddTitleContainer();
			ColorizeTitle();
			RemoveTitleLabel();
			AddGlowBorder();
			AddInputPorts(TitleContainer);
			
			_propertyContainer = CreatePropertyContainer();
			TitleContainer.Add(_propertyContainer);
			
			AddTitleIcon();
			AddCounterField();
			AddOutputPorts(TitleContainer);
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
			_field = new IntegerField("");
			_propertyContainer.Add(_field);
		}
		
		protected override void AddTitleIcon()
		{
			_icon = MakeIcon(NodeModel);
			_propertyContainer.Insert(0, _icon);
		}
	}
}