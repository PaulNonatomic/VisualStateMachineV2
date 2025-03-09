using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Manages connections between nodes in the graph view
	/// </summary>
	public class ConnectionManager
	{
		private readonly GraphView _graphView;
		private readonly NodeFinder _nodeFinder;
		private readonly PortFinder _portFinder;

		public ConnectionManager(GraphView graphView)
		{
			_graphView = graphView;
			_nodeFinder = new NodeFinder(graphView);
			_portFinder = new PortFinder();
		}

		/// <summary>
		///     Creates a connection between nodes with specified ports
		/// </summary>
		public StateNodeEdge CreateConnection(
			string originNodeId,
			PortModel originPort,
			string destinationNodeId,
			PortModel destinationPort,
			StateMachineModel model)
		{
			// First create the data model
			var transitionModel = CreateTransitionModel(
				originNodeId,
				originPort,
				destinationNodeId,
				destinationPort,
				model);

			if (transitionModel == null) return null;

			// Then create the visual edge
			return CreateEdgeView(transitionModel);
		}

		/// <summary>
		///     Creates a transition model and adds it to the state machine
		/// </summary>
		public StateTransitionModel CreateTransitionModel(
			string originNodeId,
			PortModel originPort,
			string destinationNodeId,
			PortModel destinationPort,
			StateMachineModel model)
		{
			// Validate ports compatibility
			if (!ArePortsCompatible(originPort, destinationPort))
			{
				Debug.LogWarning($"Incompatible ports: {originPort.PortTypeName} and {destinationPort.PortTypeName}");
				return null;
			}

			// Create the transition model
			var transition = new StateTransitionModel(originNodeId, originPort, destinationNodeId, destinationPort);

			// Add it to the model
			model.AddTransition(transition);

			return transition;
		}

		/// <summary>
		///     Creates a visual edge for a transition model
		/// </summary>
		public StateNodeEdge CreateEdgeView(StateTransitionModel transitionModel)
		{
			if (_graphView == null)
			{
				Debug.LogError("Cannot create edge view: graph view is null");
				return null;
			}

			// Find nodes
			var originNode = _nodeFinder.FindNodeById(transitionModel.OriginNodeId);
			if (originNode == null)
			{
				Debug.LogError($"Failed to create transition: origin node {transitionModel.OriginNodeId} not found");
				_nodeFinder.LogAvailableNodes();
				return null;
			}

			var destinationNode = _nodeFinder.FindNodeById(transitionModel.DestinationNodeId);
			if (destinationNode == null)
			{
				Debug.LogError($"Failed to create transition: destination node {transitionModel.DestinationNodeId} not found");
				return null;
			}

			// Find ports
			var outputPort = _portFinder.FindPort(originNode, transitionModel.OriginPort.Id, Direction.Output);
			if (outputPort == null)
			{
				Debug.LogError($"Failed to create transition: output port {transitionModel.OriginPort.Id} not found");
				_portFinder.LogAvailablePorts(originNode, Direction.Output);
				return null;
			}

			var inputPort = _portFinder.FindPort(destinationNode, transitionModel.DestinationPort.Id, Direction.Input);
			if (inputPort == null)
			{
				Debug.LogError($"Failed to create transition: input port {transitionModel.DestinationPort.Id} not found");
				_portFinder.LogAvailablePorts(destinationNode, Direction.Input);
				return null;
			}

			// Create the edge
			var edge = new StateNodeEdge
			{
				input = inputPort,
				output = outputPort,
				userData = transitionModel,
				name = GenerateEdgeName(originNode.name, destinationNode.name)
			};

			// Position the edge
			var position = edge.GetPosition();
			position.position -= (Vector2)_graphView.contentViewContainer.transform.position;
			edge.SetPosition(position);

			// Connect ports and add to graph
			inputPort.Connect(edge);
			outputPort.Connect(edge);
			_graphView.AddElement(edge);

			return edge;
		}

		/// <summary>
		///     Removes a connection from the graph
		/// </summary>
		public void RemoveConnection(StateNodeEdge edge, StateMachineModel model)
		{
			if (_graphView == null || edge == null) return;

			// Get transition model
			var transitionModel = edge.userData as StateTransitionModel;
			if (transitionModel == null) return;

			// Remove from model
			model.RemoveTransition(transitionModel);

			// Disconnect from ports
			edge.input?.Disconnect(edge);
			edge.output?.Disconnect(edge);

			// Remove from graph
			_graphView.RemoveElement(edge);
		}

		private bool ArePortsCompatible(PortModel originPort, PortModel destinationPort)
		{
			// Empty port types are compatible with anything
			var blankPortTypes = string.IsNullOrEmpty(originPort.PortTypeName) &&
								 string.IsNullOrEmpty(destinationPort.PortTypeName);
			if (blankPortTypes) return true;

			// Otherwise types must match
			return originPort.PortTypeName == destinationPort.PortTypeName;
		}

		private string GenerateEdgeName(string originNodeName, string destinationNodeName)
		{
			return $"Edge-{originNodeName}-{destinationNodeName}";
		}
	}
}