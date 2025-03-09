using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Responsible for finding ports on nodes
	/// </summary>
	public class PortFinder
	{
		/// <summary>
		///     Finds a port on a node by ID and direction, using multiple search strategies
		/// </summary>
		public Port FindPort(NodeView node, string portId, Direction direction)
		{
			var container = direction == Direction.Input ? node.inputContainer : node.outputContainer;

			// Strategy 1: Direct name match
			var port = container.Q<Port>(portId);
			if (port != null) return port;

			// Strategy 2: Search all ports in container
			var allPorts = container.Query<Port>().ToList();

			foreach (var p in allPorts)
			{
				// Check direct name match
				if (p.name == portId) return p;

				// Check userData
				if (p.userData is PortModel model && model.Id == portId) return p;
			}

			// Strategy 3: Full node search
			return node.Query<Port>().Where(p =>
			{
				var nameMatch = p.name == portId;
				var userDataMatch = p.userData is PortModel model && model.Id == portId;
				var directionMatch = p.direction == direction;

				return (nameMatch || userDataMatch) && directionMatch;
			});
		}

		/// <summary>
		///     Logs all ports on a node for debugging
		/// </summary>
		public void LogAvailablePorts(NodeView node, Direction direction)
		{
			var container = direction == Direction.Input ? node.inputContainer : node.outputContainer;
			var allPorts = container.Query<Port>().ToList();

			Debug.Log($"Available {direction} ports on {node.name}:");
			foreach (var port in allPorts)
			{
				var portModelId = port.userData is PortModel pm ? pm.Id : "no-model";
				Debug.Log($"  - Port name: {port.name}, portName: {port.portName}, userData ID: {portModelId}");
			}
		}
	}
}