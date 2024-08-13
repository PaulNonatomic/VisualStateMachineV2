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
			
			AddStyle(nameof(EntryNodeView));
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel();
			AddGlowBorder();
			AddTitleIcon();
			AddOutputPorts(TitleContainer);
			UpdatePosition();
		}
		
		public override void Update()
		{
			UpdateGlowBorder();
		}
	}
}