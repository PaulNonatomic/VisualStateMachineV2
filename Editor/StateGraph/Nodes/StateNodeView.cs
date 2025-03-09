using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class StateNodeView : BaseStateNodeView
	{
		public StateNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			var contents = this.Q<VisualElement>("contents");

			StyleManager.AddStyleSheet(nameof(StateNodeView));
			StyleManager.AddTitleLabel();

			var icon = StyleManager.CreateNodeIcon();
			StyleManager.TitleContainer.Insert(0, icon);

			AddEditButton();
			AnimationController.AddProgressBar();
			PortManager.AddInputPorts(inputContainer);
			PortManager.AddOutputPorts(outputContainer);

			var propertyContainer = PropertyPanel.CreatePropertyContainer();
			propertyContainer.AddToClassList("full-width");
			contents.Insert(0, propertyContainer);

			PropertyPanel.AddProperties(propertyContainer);
			AnimationController.AddGlowBorder();
			StyleManager.ApplyNodeWidth();
			UpdatePosition();
		}

		public override void Update()
		{
			AnimationController.UpdateAnimations();
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