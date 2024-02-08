using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public class EntryNodeView : BaseStateNodeView
	{
		public EntryNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel,  
							 StateNodeModel nodeModel) 
							: base(graphView, stateMachineModel, nodeModel)
		{
			
			AddStyle("EntryNodeView");
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