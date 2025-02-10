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
			
			StateGraphTransitionFactory.MakeTransitionView(graphView, transitionData);
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
			if (originNode == null)
			{
				Debug.LogWarning($"Failed to create transition because of missing origin node with id:{transitionModel.OriginNodeId}");
				return null;
			}
				
			var destinationNode = graphView.contentViewContainer.Q<NodeView>(transitionModel.DestinationNodeId);
			if (destinationNode == null)
			{
				Debug.LogWarning($"Failed to create transition because of missing destination node with id:{transitionModel.DestinationNodeId}");
				return null;
			}
				
			var inputPort = destinationNode.Q<Port>(transitionModel.DestinationPort.Id, "port", "input");
			if (inputPort == null)
			{
				Debug.LogWarning($"Failed to create transition because of missing input port with id:{transitionModel.DestinationPort.Id}");
				return null;
			}
				
			var outputPort = originNode.Q<Port>(transitionModel.OriginPort.Id, "port", "output");
			if (outputPort == null)
			{
				Debug.LogWarning($"Failed to create transition because of missing output port with id:{transitionModel.OriginPort.Id}");
				return null;
			}

			var edge = new StateNodeEdge()
			{
				input = inputPort,
				output = outputPort,
				userData = transitionModel,
				name = GenerateEdgeName(inputPort, outputPort)
			};
		
			var position = edge.GetPosition();
			position.position -= (Vector2) graphView.contentViewContainer.transform.position;
			edge.SetPosition(position);
			
			inputPort.Connect(edge);
			outputPort.Connect(edge); 
			graphView.AddElement(edge);

			return edge;
		}

		public static string GenerateEdgeName(TransitionModel model)
		{
			return  $"Edge-{model.OriginNodeId}-{model.DestinationNodeId}";
		}
		
		public static string GenerateEdgeName(Port inputPort, Port outputPort)
		{
			return  $"Edge-{inputPort.node.name}-{outputPort.node.name}";
		}
	}
}