using System.Linq;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.Utils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nonatomic.VSM2.StateGraph.Validation
{
	public static class StateMachineValidator
	{
		public static void Validate(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			MigrateNodePorts(stateMachineModel);
			MigrateTransitions(stateMachineModel);
			
			ValidateNodes(stateMachineModel);
			ValidateNodePorts(stateMachineModel);
			ValidateTransitions(stateMachineModel);
		}

		private static void ValidateNodes(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			var invalidNodes = stateMachineModel.Nodes.Where(node => !node.State).ToList();

			foreach (var node in invalidNodes)
			{
				var removed = stateMachineModel.TryRemoveNode(node);
				GraphLog.LogWarning(removed
					? $"Removed invalid node {node.Id}"
					: $"Could not remove invalid node {node.Id}");
			}
		}

		private static void ValidateNodePorts(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			var changesMade = false;

			foreach (var node in stateMachineModel.Nodes)
			{
				changesMade |= StateNodeValidator.ValidateOutputPorts(node);
				changesMade |= StateNodeValidator.ValidateInputPorts(node);
			}

			#if UNITY_EDITOR
			{
				if (changesMade)
				{
					EditorUtility.SetDirty(stateMachineModel);
				}
			}
			#endif
		}

		private static void ValidateTransitions(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			for (var index = stateMachineModel.Transitions.Count - 1; index >= 0; index--)
			{
				var transition = stateMachineModel.Transitions[index];
				
				// Handle migration (Older state machines had an OnEnterState method)
				if (transition.DestinationNodeId == "OnEnterState")
				{
					transition.DestinationNodeId = "OnEnter";
				}
				
				var originNode = stateMachineModel.Nodes.FirstOrDefault(node => node.Id == transition.OriginNodeId);
				var destinationNode = stateMachineModel.Nodes.FirstOrDefault(node => node.Id == transition.DestinationNodeId);
				
				if (originNode != null && destinationNode != null)
				{
					if (NodeGraphModelUtils.TryGetPortsByIdWithIndexFallback(transition, originNode, destinationNode,
						out var originPort, out var destinationPort))
					{
						NodeGraphModelUtils.SyncTransitionPorts(transition, originPort, destinationPort);
						continue;
					}
				}

				GraphLog.LogWarning(stateMachineModel.TryRemoveTransition(transition)
					? $"Removed invalid transition {transition}"
					: $"Could not remove invalid transition {transition}");
			}
		}
		
		private static void MigrateTransitions(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			for (var index = stateMachineModel.Transitions.Count - 1; index >= 0; index--)
			{
				var transition = stateMachineModel.Transitions[index];
				
				// Handle migration (Older state machines had an OnEnterState method)
				if (transition.DestinationNodeId == "OnEnterState")
				{
					transition.DestinationNodeId = "OnEnter";
				}
			}
		}
		
		private static void MigrateNodePorts(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			foreach (var node in stateMachineModel.Nodes)
			{
				foreach (var port in node.InputPorts)
				{
					if(port.Id == "OnEnterState")
					{
						port.Id = "OnEnter";
					}
				}

				foreach (var port in node.OutputPorts)
				{
					if(port.Id == "OnEnterState")
					{
						port.Id = "OnEnter";
					}
				}
			}
		}
	}
}
