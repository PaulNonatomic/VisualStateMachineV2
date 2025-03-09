using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class CounterNodeView : BaseStateNodeView
	{
		private readonly BaseCounterState _counterState;
		private IntegerField _field;
		private Image _icon;
		private VisualElement _propertyContainer;

		public CounterNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
			_counterState = (BaseCounterState)nodeModel.State;
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			StyleManager.AddStyleSheet(nameof(CounterNodeView));

			// Add "compact-node" class for width control
			AddToClassList("compact-node");

			StyleManager.RemoveTitleLabel();
			AnimationController.AddGlowBorder();
			PortManager.AddInputPorts(StyleManager.TitleContainer);

			_propertyContainer = PropertyPanel.CreatePropertyContainer();
			_propertyContainer.AddToClassList("compact-container"); // Add class for styling
			StyleManager.TitleContainer.Add(_propertyContainer);

			AddTitleIcon();
			AddCounterField();
			PortManager.AddOutputPorts(StyleManager.TitleContainer);

			// Explicitly set width
			style.width = 120;
			style.minWidth = 120;
			style.maxWidth = 120;

			UpdatePosition();
		}

		public override void Update()
		{
			AnimationController.UpdateAnimations();
			UpdateField();
		}

		private void UpdateField()
		{
			_field.value = _counterState.Count;
		}

		private void AddCounterField()
		{
			_field = new IntegerField("");
			_field.isReadOnly = true;
			_field.AddToClassList("count-field");
			_field.style.minWidth = 40;
			_field.style.maxWidth = 60;

			_propertyContainer.Add(_field);
		}

		private void AddTitleIcon()
		{
			_icon = StyleManager.CreateNodeIcon();
			_icon.style.width = 16;
			_icon.style.height = 16;
			_propertyContainer.Insert(0, _icon);
		}
	}
}