using System.Linq;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Responsible for finding nodes in a graph
	/// </summary>
	public class NodeFinder
	{
		private readonly GraphView _graphView;

		public NodeFinder(GraphView graphView)
		{
			_graphView = graphView;
		}

		/// <summary>
		///     Finds a node by its ID, using multiple search strategies
		/// </summary>
		public NodeView FindNodeById(string nodeId)
		{
			// Strategy 1: Direct find by name
			var node = _graphView.nodes.ToList().FirstOrDefault(n => n.name == nodeId) as NodeView;
			if (node != null) return node;

			// Strategy 2: Find by userData ID
			node = _graphView.nodes.ToList().FirstOrDefault(n =>
			{
				if (n.userData is StateNodeModel model) return model.Id == nodeId;
				return false;
			}) as NodeView;

			if (node == null) Debug.LogWarning($"Node {nodeId} not found by any search method");

			return node;
		}

		/// <summary>
		///     Lists all nodes in the graph for debugging
		/// </summary>
		public void LogAvailableNodes()
		{
			Debug.Log("Available nodes in graph:");
			foreach (var node in _graphView.nodes) Debug.Log($"  - Node: {node.name}");
		}
	}
}