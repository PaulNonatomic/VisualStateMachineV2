using System.Linq;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.Utils;
using UnityEditor;

namespace Nonatomic.VSM2.StateGraph.Validation
{
	public static class StateMachineValidator
	{
		public static void Validate(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

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
				bool removed = stateMachineModel.TryRemoveNode(node);
				GraphLog.LogWarning(removed
					? $"Removed invalid node {node.Id}"
					: $"Could not remove invalid node {node.Id}");
			}
		}

		private static void ValidateNodePorts(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			bool changesMade = false;

			foreach (var node in stateMachineModel.Nodes)
			{
				changesMade |= StateNodeValidator.ValidateOutputPorts(node);
				changesMade |= StateNodeValidator.ValidateInputPorts(node);
			}

#if UNITY_EDITOR
			if (changesMade)
			{
				EditorUtility.SetDirty(stateMachineModel);
			}
#endif
		}

		private static void ValidateTransitions(StateMachineModel stateMachineModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			for (var index = stateMachineModel.Transitions.Count - 1; index >= 0; index--)
			{
				var transition = stateMachineModel.Transitions[index];
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
	}
}
