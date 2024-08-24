using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Nodes
{
	public sealed class AnyNodeView : BaseStateNodeView
	{
		public AnyNodeView(GraphView graphView, 
							 StateMachineModel stateMachineModel,  
							 StateNodeModel nodeModel) 
							: base(graphView, stateMachineModel, nodeModel)
		{
			
			AddStyle(nameof(AnyNodeView));
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
		
		protected override void AddOutputPorts(VisualElement portContainer)
		{
			foreach (var t in NodeModel.OutputPorts)
			{
				var portData = t;
				TryUpdatePortDataFromState(NodeModel, portData.Id, out portData);

				StateGraphPortFactory.MakePort(GraphView, 
					StateMachineModel,
					this,
					portContainer, 
					Direction.Output, 
					Port.Capacity.Multi, 
					portData);
			}
		}
	}
}