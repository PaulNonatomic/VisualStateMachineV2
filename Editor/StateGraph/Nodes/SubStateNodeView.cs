using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class SubStateNodeView : BaseStateNodeView
	{
		public SubStateNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			StyleManager.AddStyleSheet(nameof(SubStateNodeView));
			StyleManager.AddTitleLabel();

			var icon = StyleManager.CreateNodeIcon();
			StyleManager.TitleContainer.Insert(0, icon);

			AnimationController.AddProgressBar();
			PortManager.AddInputPorts(inputContainer);
			PortManager.AddOutputPorts(outputContainer);

			var contents = this.Q<VisualElement>("contents");
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
	}
}