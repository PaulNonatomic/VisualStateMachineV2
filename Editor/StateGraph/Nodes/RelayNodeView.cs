using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class RelayNodeView : BaseStateNodeView
	{
		public RelayNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel, 
							 StateNodeModel nodeModel) 
							: base(graphView, stateMachineModel, nodeModel)
		{
			AddStyle(nameof(RelayNodeView));
			AddTitleContainer();
			ColorizeTitle();
			RemoveTitleLabel();
			AddGlowBorder();
			AddInputPorts(TitleContainer);
			AddOutputPorts(TitleContainer);
			UpdatePosition();
		}
		
		public override void Update()
		{
			UpdateGlowBorder();
		}
	}
}