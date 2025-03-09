using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class CounterWithTargetNodeView : BaseStateNodeView
	{
		private readonly BaseCounterState _counterState;
		private IntegerField _field;
		private VisualElement _propertyContainer;

		public CounterWithTargetNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
			_counterState = (BaseCounterState)nodeModel.State;
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			var contents = this.Q<VisualElement>("contents");

			StyleManager.AddStyleSheet(nameof(CounterWithTargetNodeView));
			StyleManager.AddTitleLabel();

			var icon = StyleManager.CreateNodeIcon();
			StyleManager.TitleContainer.Insert(0, icon);

			AddEditButton();
			AnimationController.AddProgressBar();
			PortManager.AddInputPorts(inputContainer);
			PortManager.AddOutputPorts(outputContainer);

			_propertyContainer = PropertyPanel.CreatePropertyContainer();
			_propertyContainer.AddToClassList("full-width");
			contents.Insert(0, _propertyContainer);

			PropertyPanel.AddProperties(_propertyContainer);
			AddCounterField();
			AnimationController.AddGlowBorder();
			StyleManager.ApplyNodeWidth();
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
			_field = new IntegerField("Count");
			_field.isReadOnly = true;
			_field.AddToClassList("count-field");

			var scroll = _propertyContainer.Q<ScrollView>();
			scroll.Insert(0, _field);
		}

		private void AddEditButton()
		{
			var editButton = CreateEditButton(HandleEditButton);
			if (editButton != null) StyleManager.TitleContainer.Add(editButton);
		}

		private void HandleEditButton()
		{
			OpenStateScript();
		}
	}
}