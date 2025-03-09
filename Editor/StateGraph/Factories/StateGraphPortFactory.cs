using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Factory for creating port interfaces on nodes
	/// </summary>
	public static class StateGraphPortFactory
	{
		/// <summary>
		///     Creates a port on a node and adds it to the specified container
		/// </summary>
		public static Port MakePort(
			GraphView graphView,
			StateMachineModel model,
			NodeView nodeView,
			VisualElement parent,
			Direction direction,
			Port.Capacity capacity,
			PortModel portModel)
		{
			// Create an edge connector listener
			var stateNodeEdgeListener = new StateNodeEdgeListener(graphView, model);

			// Use PortOperations to create the port
			var port = PortOperations.CreatePort(
				nodeView,
				direction,
				capacity,
				portModel,
				stateNodeEdgeListener);

			// Add to container
			PortOperations.AddPortToContainer(port, parent);

			return port;
		}
	}
}