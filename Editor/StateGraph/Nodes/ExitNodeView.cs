using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class ExitNodeView : BaseStateNodeView
	{

		public ExitNodeView(GraphView graphView, 
							StateMachineModel stateMachineModel,  
							StateNodeModel nodeModel) 
							: base(graphView, stateMachineModel, nodeModel)
		{
			AddStyle("ExitNodeView");
			AddTitleContainer();
			ColorizeTitle();
			AddTitleLabel();
			AddGlowBorder();
			AddTitleIcon();
			AddInputPorts(TitleContainer);
			UpdatePosition();
		}
		
		public override void Update()
		{
			UpdateGlowBorder();
		}
	}
}