using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public static class StateGraphTransitionFactory
	{
		public static StateTransitionModel MakeTransitionData(StateMachineModel model, 
															  string originNodeId, 
															  PortModel originPort, 
															  string destinationNodeId, 
															  PortModel destinationPort)
		{
			var transition = new StateTransitionModel(originNodeId, originPort, destinationNodeId, destinationPort);
			model.AddTransition(transition);
			return transition;
		}
		
		public static void MakeTransition(GraphView graphView, 
										  StateMachineModel stateMachineModel, 
										  string originNodeId, 
										  PortModel originPortModel, 
										  string destinationNodeId, 
										  PortModel destinationPortModel)
		{
			var transitionData = StateGraphTransitionFactory.MakeTransitionData(stateMachineModel, 
																				originNodeId, 
																				originPortModel, 
																				destinationNodeId, 
																				destinationPortModel);
			
			var transitionView = StateGraphTransitionFactory.MakeTransitionView(graphView, transitionData);
		}
		
		public static StateTransitionModel MakeTransitionData(Edge edge)
		{
			var originNodeId = edge.output.node.name;
			var originPort = edge.output.userData as PortModel;
			
			var destinationNodeId = edge.input.node.name;
			var destinationPort = edge.input.userData as PortModel;
			
			return new StateTransitionModel(originNodeId, originPort, destinationNodeId, destinationPort);
		}
		
		public static StateNodeEdge MakeTransitionView(GraphView graphView, StateTransitionModel transitionModel)
		{
			var originNode = graphView.contentViewContainer.Q<NodeView>(transitionModel.OriginNodeId);
			if(originNode == null) throw new Exception("Failed to create edge because of missing origin node");
				
			var destinationNode = graphView.contentViewContainer.Q<NodeView>(transitionModel.DestinationNodeId);
			if(destinationNode == null) throw new Exception("Failed to create edge because of missing destination node");
				
			var inputPort = destinationNode.Q<Port>(transitionModel.DestinationPort.Id, "port", "input");
			if(inputPort == null) throw new Exception("Failed to create edge because of missing input port");
				
			var outputPort = originNode.Q<Port>(transitionModel.OriginPort.Id, "port", "output");
			if(outputPort == null) throw new Exception("Failed to create edge because of missing output port");

			var frameDelay = transitionModel.OriginPort.FrameDelay;
			Debug.Log($"{originNode.name}, {destinationNode.name}, {transitionModel.OriginPort.FrameDelay}, {transitionModel.DestinationPort.FrameDelay}");
			var edge = new StateNodeEdge()
			{
				FrameDelay = frameDelay,
				input = inputPort,
				output = outputPort,
				userData = transitionModel
			};
		
			var position = edge.GetPosition();
			position.position -= (Vector2) graphView.contentViewContainer.transform.position;
			edge.SetPosition(position);
			
			inputPort.Connect(edge);
			outputPort.Connect(edge); 
			graphView.AddElement(edge);

			return edge;
		}
	}
}