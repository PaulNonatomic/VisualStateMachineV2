using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class RelayNodeView : BaseStateNodeView
	{
		public RelayNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			StyleManager.AddStyleSheet(nameof(RelayNodeView));
			StyleManager.RemoveTitleLabel();
			AnimationController.AddGlowBorder();
			PortManager.AddInputPorts(StyleManager.TitleContainer);
			PortManager.AddOutputPorts(StyleManager.TitleContainer);
			UpdatePosition();
		}

		public override void Update()
		{
			AnimationController.UpdateAnimations();
		}
	}
}