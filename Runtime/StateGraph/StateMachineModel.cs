using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.Utils;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	[CreateAssetMenu(fileName = "StateMachineModel", menuName = Constants.AssetMenuRoot + "/StateMachineModel")]
	public class StateMachineModel : NodeGraphModel<StateNodeModel, StateTransitionModel>
	{
		public StateMachineModel Original { get; private set; }
		public StateMachineModel Parent { get; private set; }
		public string ModelName => Original == null ? name : Original.name;
		
		public bool HasState<T>() where T : State
		{
			var stateNodes = Nodes.Cast<StateNodeModel>();
			return stateNodes.Any(node => node.State is T);
		}

		public static StateMachineModel CreateInstance(StateMachineModel model)
		{
			var instance = Instantiate(model);
			instance.name = instance.name;// + instance.GetInstanceID();
			instance.Original = model;
			
			return instance;
		}

		public void AddState(StateNodeModel stateNodeModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation() || stateNodeModel == null) return;
			
			#if UNITY_EDITOR
			{
				EditorApplication.delayCall += () =>
				{
					if (this == null) return;

					if (TryAddNode(stateNodeModel))
					{
						AddSubAsset(stateNodeModel.State);
					}
				};
			}
			#endif
		}

		public void RemoveState(StateNodeModel stateNodeModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation() || stateNodeModel == null) return;

			#if UNITY_EDITOR
			{
				EditorApplication.delayCall += () =>
				{
					if (this == null) return;

					if (TryRemoveNode(stateNodeModel))
					{
						RemoveSubAsset(stateNodeModel.State);
					}

					ValidateSubAssets();
				};
			}
			#endif
		}

		public void AddTransition(StateTransitionModel stateTransitionModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation() || stateTransitionModel == null) return;

			#if UNITY_EDITOR
			{
				EditorApplication.delayCall += () =>
				{
					if (this == null) return;

					if (!TryAddTransition(stateTransitionModel))
					{
						GraphLog.LogWarning("Failed to add transition");
					}
				};
			}
			#endif
		}
		
		public void RemoveTransition(StateTransitionModel stateTransitionModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation() || stateTransitionModel == null) return;

			#if UNITY_EDITOR
			{
				EditorApplication.delayCall += () =>
				{
					if (this == null) return;

					if (!TryRemoveTransition(stateTransitionModel))
					{
						GraphLog.LogWarning("Failed to remove transition");
					}

					ValidateSubAssets();
				};
			}
			#endif
		}

		protected override void ValidateSubAssets()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			base.ValidateSubAssets();

			ValidateNodes();
			ValidateNodePorts();
			ValidateTransitions();
		}

		private void ValidateTransitions()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			for (var index = Transitions.Count - 1; index >= 0; index--)
			{
				var transition = Transitions[index];
				var originNode = Nodes.FirstOrDefault(node => node.Id == transition.OriginNodeId);
				var destinationNode = Nodes.FirstOrDefault(node => node.Id == transition.DestinationNodeId);

				if (originNode != null && destinationNode != null)
				{
					if (NodeGraphModelUtils.TryGetPortsByIdWithIndexFallback(transition, originNode, destinationNode,
														 out var originPort, out var destinationPort))
					{
						NodeGraphModelUtils.SyncTransitionPorts(transition, originPort, destinationPort);
						continue;
					}
				}

				GraphLog.LogWarning(TryRemoveTransition(transition)
					? $"Removed invalid transition {transition}"
					: $"Could not remove invalid transition {transition}");
			}
		}

		private void ValidateNodePorts()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			foreach (var node in Nodes)
			{
				var stateNode = node as StateNodeModel;
				stateNode.ValidatePorts();
			}
		}

		private void ValidateNodes()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			var invalidNodes = new List<StateNodeModel>();
			
			foreach (var node in Nodes)
			{
				var stateNode = node as StateNodeModel;
				if (stateNode.State == null) invalidNodes.Add(stateNode);
			}
			
			foreach (var node in invalidNodes)
			{
				Debug.LogWarning(TryRemoveNode(node)
					? $"Removed invalid node {node}"
					: $"Could not remove invalid node {node}");
			}
		}

		public void Initialize(GameObject gameObject, StateMachine stateMachine)
		{
			if (gameObject == null || stateMachine == null) return;

			foreach (var stateNode in Nodes)
			{
				if (stateNode == null || stateNode.State == null) continue;

				var instantiatedState = Instantiate(stateNode.State);
				if (instantiatedState == null) continue;
				
				instantiatedState.GameObject = gameObject;
				instantiatedState.StateMachine = stateMachine;
				stateNode.State = instantiatedState;
				stateNode.Awake();
			}
		}

		public bool TryGetNodeByState<T>(out StateNodeModel stateNodeModel) where T : State
		{
			stateNodeModel = Nodes.FirstOrDefault(node => node != null && node.State is T);
			return stateNodeModel != null;
		}

		public void SetParent(StateMachineModel stateMachineModel)
		{
			if (stateMachineModel == this) return;
			
			Parent = stateMachineModel;
		}
	}
}