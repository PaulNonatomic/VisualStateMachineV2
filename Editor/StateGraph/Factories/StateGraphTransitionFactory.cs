using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Factory for creating transitions between nodes
	///     This class serves as a façade for the connection management system
	/// </summary>
	public static class StateGraphTransitionFactory
	{
		/// <summary>
		///     Creates a transition between nodes and adds it to the model and graph view
		/// </summary>
		public static void MakeTransition(
			GraphView graphView,
			StateMachineModel stateMachineModel,
			string originNodeId,
			PortModel originPortModel,
			string destinationNodeId,
			PortModel destinationPortModel)
		{
			var connectionManager = new ConnectionManager(graphView);
			connectionManager.CreateConnection(
				originNodeId,
				originPortModel,
				destinationNodeId,
				destinationPortModel,
				stateMachineModel);
		}

		/// <summary>
		///     Creates a transition model and adds it to the state machine
		/// </summary>
		public static StateTransitionModel MakeTransitionData(
			StateMachineModel model,
			string originNodeId,
			PortModel originPort,
			string destinationNodeId,
			PortModel destinationPort)
		{
			var connectionManager = new ConnectionManager(null);
			return connectionManager.CreateTransitionModel(
				originNodeId,
				originPort,
				destinationNodeId,
				destinationPort,
				model);
		}

		/// <summary>
		///     Creates a visual edge for a transition model
		/// </summary>
		public static StateNodeEdge MakeTransitionView(GraphView graphView, StateTransitionModel transitionModel)
		{
			var connectionManager = new ConnectionManager(graphView);
			return connectionManager.CreateEdgeView(transitionModel);
		}
	}
}