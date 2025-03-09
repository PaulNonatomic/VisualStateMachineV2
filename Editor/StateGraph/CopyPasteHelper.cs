using System;
using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Persistence;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.Editor.StateGraph.VisualElements;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public static class CopyPasteHelper
	{
		public static CopiedData LastCopy => _lastCopy;
		private static CopiedData _lastCopy;
		
		public static void CacheCopiedData(CopiedData copy)
		{
			_lastCopy = copy;
		}

		public static void ClearCopyCache()
		{
			_lastCopy = null;
		}

		public static void Copy(StateGraphView graphView)
		{
			var selectedNodeModels = graphView.selection
				.OfType<BaseStateNodeView>()
				.Select(node => node.NodeModel)
				.ToList();
			
			var selectedTransitions = graphView.selection
				.OfType<StateNodeEdge>()
				.Select(edge => (StateTransitionModel) edge.userData)
				.ToList();

			var copy = new CopiedData(selectedNodeModels, selectedTransitions);
			CacheCopiedData(copy);
		}

		public static void Paste(StateGraphView graphView)
		{
			if (LastCopy == null) return;

			var model = graphView.StateManager.Model as StateMachineModel;
			if (!model) return;

			var clonedNodes = LastCopy.SelectedNodes.Select(node => node.Clone()).ToList();
			var clonedTransition = LastCopy.SelectedTransitions.Select(trans => trans.Clone()).ToList();

			RenameClonedNodes(model, clonedNodes);
			OffsetNodes(clonedNodes, new Vector2(50, 50));
			
			//Remap transition nodes since their GUIDs have changed
			RemapTransitionNodes(graphView, model, LastCopy, clonedTransition, clonedNodes);

			graphView.PopulateGraph(model);

			//Wait a frame for the nodes to be created then select them
			EditorApplication.delayCall += ()=> SelectNodes(graphView, clonedNodes);
		}

		private static void SelectNodes(StateGraphView graphView, List<StateNodeModel> nodes)
		{
			graphView.selection.Clear();
				
			foreach (var node in nodes)
			{
				var originNode = graphView.contentViewContainer.Q<NodeView>(node.Id);
				originNode?.Select(graphView, true);
			}
		}

		private static void RenameClonedNodes(StateMachineModel model, List<StateNodeModel> clonedNodes)
		{
			foreach (var node in clonedNodes)
			{
				//Prevent copying entry nodes
				if (node.State is EntryState) continue;

				//Provide new GUIDs for the pasted node models
				node.Id = node.State.name = StateGraphNodeFactory.GenerateStateName(node.State.GetType());

				model.AddState(node);
			}
		}

		private static void OffsetNodes(List<StateNodeModel> nodes, Vector2 offset)
		{
			foreach (var node in nodes)
			{
				node.Position += offset;
			}
		}

		private static void RemapTransitionNodes(StateGraphView graphView, StateMachineModel model, CopiedData copy, List<StateTransitionModel> clonedTransition, List<StateNodeModel> clonedNodes)
		{
			foreach (var transition in clonedTransition)
			{
				UpdateNode(graphView, copy, transition, clonedNodes, true);
				UpdateNode(graphView, copy, transition, clonedNodes, false);
				
				if (transition.OriginNodeId != null && transition.DestinationNodeId != null)
				{
					model.AddTransition(transition);
				}
			}
		}
		
		private static void UpdateNode(StateGraphView graphView, CopiedData copy, TransitionModel transition, List<StateNodeModel> clonedNodes, bool updateOrigin)
		{
			// Determine the node type to update based on the parameter
			var nodeId = updateOrigin 
				? transition.OriginNodeId 
				: transition.DestinationNodeId;
			
			Func<StateNodeModel, List<PortModel>> portSelector = updateOrigin
				? node => node.OutputPorts 
				: node => node.InputPorts;

			var nodeIndex = copy.SelectedNodes.FindIndex(node => node.Id.Equals(nodeId));
			if (nodeIndex > -1)
			{
				// Node was found in copied nodes, update from cloned nodes
				var clonedNode = clonedNodes[nodeIndex];
				if (updateOrigin)
				{
					transition.OriginNodeId = clonedNode.Id;
					transition.OriginPort = clonedNode.OutputPorts.FirstOrDefault(port => port.Id.Equals(transition.OriginPort.Id));
				}
				else
				{
					transition.DestinationNodeId = clonedNode.Id;
					transition.DestinationPort = clonedNode.InputPorts.FirstOrDefault(port => port.Id.Equals(transition.DestinationPort.Id));
				}
			}
			else
			{
				// Node was not found in copied nodes, find in existing nodes
				var existingNodeView = graphView.contentViewContainer.Q<BaseStateNodeView>(nodeId);
				if (existingNodeView == null) return;

				if (updateOrigin)
				{
					transition.OriginNodeId = existingNodeView.NodeModel.Id;
					transition.OriginPort = existingNodeView.NodeModel.OutputPorts.FirstOrDefault(port => port.Id.Equals(transition.OriginPort.Id));
				}
				else
				{
					transition.DestinationNodeId = existingNodeView.NodeModel.Id;
					transition.DestinationPort = existingNodeView.NodeModel.InputPorts.FirstOrDefault(port => port.Id.Equals(transition.DestinationPort.Id));
				}
			}
		}
	}
}