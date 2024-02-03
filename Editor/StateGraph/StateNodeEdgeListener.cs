using System.Linq;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateNodeEdgeListener : IEdgeConnectorListener
	{
		private readonly StateMachineModel _stateMachineModel;
		private readonly GraphView _graphView;

		public StateNodeEdgeListener(GraphView graphView, StateMachineModel stateMachineModel)
		{
			_graphView = graphView;
			_stateMachineModel = stateMachineModel;
		}
		
		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			if (Application.isPlaying) return;
			OnDropOutsideOutputPort(edge, position);
		}
		
		private void OnDropOutsideOutputPort(Edge edge, Vector2 position)
		{
			if (Application.isPlaying) return;
			if (edge.output == null) return;
			if (edge.output.capacity == Port.Capacity.Single && edge.output.connections.ToList().Count > 0) return;
			
			var droppedEdgeOutput = edge.output;
			var droppedEdgeUserData = edge.output.userData;
			var mousePosition = Event.current.mousePosition;
			var screenPosition = GUIUtility.GUIToScreenPoint(mousePosition);
			var nodePosition = GraphUtils.ScreenPointToGraphPoint(position, _graphView);
			
			StateSelectorWindow.Open(_stateMachineModel, screenPosition, stateType =>
			{
				var nodeData = StateGraphFactory.MakeStateNodeData(_stateMachineModel, stateType, nodePosition);
				var nodeView = StateGraphFactory.MakeNode(_graphView, nodeData, _stateMachineModel);
				
				var originNodeId = droppedEdgeOutput.node.name;
				var originPortData = droppedEdgeUserData as PortModel;
				var destinationNodeId = nodeData.Id;
				var destinationPortData = nodeData.InputPorts[0];
				
				StateGraphFactory.MakeTransition(_graphView, _stateMachineModel, originNodeId, originPortData, 
							destinationNodeId, destinationPortData);
			});
		}

				public void OnDrop(GraphView graphView, Edge edge)
		{
			if (Application.isPlaying) return;
			if (edge.output == null || edge.input == null) return;
			
			var port = edge.output;
			var inputNode = edge.input.node as NodeView;

			var originNodeId = edge.output.node.name;
			var originPortModel = edge.output.userData as PortModel;
			
			var destinationNodeId = edge.input.node.name;
			var destinationPortModel = edge.input.userData as PortModel;
			
			StateGraphFactory.MakeTransition(_graphView, _stateMachineModel, originNodeId, originPortModel, 
				destinationNodeId, destinationPortModel);
		}
	}
}