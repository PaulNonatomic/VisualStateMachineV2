using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class EntryNodeView : BaseStateNodeView
	{
		public EntryNodeView(GraphView graphView,
			StateMachineModel stateMachineModel,
			StateNodeModel nodeModel)
			: base(graphView, stateMachineModel, nodeModel)
		{
		}

		protected override void InitializeNode()
		{
			base.InitializeNode();

			StyleManager.AddStyleSheet(nameof(EntryNodeView));
			StyleManager.AddTitleLabel();
			AnimationController.AddGlowBorder();

			var icon = StyleManager.CreateNodeIcon();
			StyleManager.TitleContainer.Insert(0, icon);

			PortManager.AddOutputPorts(StyleManager.TitleContainer);
			UpdatePosition();
		}

		public override void Update()
		{
			AnimationController.UpdateAnimations();
		}
	}
}