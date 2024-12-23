using System.Linq;
using Nonatomic.VSM2.Data;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Validation;
using Nonatomic.VSM2.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nonatomic.VSM2.StateGraph
{
	[CreateAssetMenu(fileName = "StateMachineModel", menuName = Constants.AssetMenuRoot + "/StateMachineModel")]
	public class StateMachineModel : NodeGraphModel<StateNodeModel, StateTransitionModel>
	{
		public StateMachineModel Original { get; private set; }
		public StateMachineModel Parent { get; private set; }
		public string ModelName => !Original ? name : Original.name;
		
		public bool HasState<T>() where T : State
		{
			var stateNodes = Nodes.Cast<StateNodeModel>();
			return stateNodes.Any(node => node.State is T);
		}

		public static StateMachineModel CreateInstance(StateMachineModel model)
		{
			var instance = Instantiate(model);
			instance.name = instance.name;
			instance.Original = model;
			
			return instance;
		}

		/**
		 * Will validate the subassets of the model asset
		 */
		public void SelfValidate()
		{
			StateMachineValidator.Validate(this);
		}

		public void AddState(StateNodeModel stateNodeModel)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation() || stateNodeModel == null) return;
			
			#if UNITY_EDITOR
			{
				EditorApplication.delayCall += () =>
				{
					if (!this) return;
			
					if (TryAddNode(stateNodeModel))
					{
						AddSubAsset(stateNodeModel.State);
					}
					
					ValidateSubAssets();
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
					if (!this) return;
			
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
					if (!this) return;
			
					if (!TryAddTransition(stateTransitionModel))
					{
						GraphLog.LogWarning("Failed to add transition");
					}
					
					ValidateSubAssets();
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
					if (!this) return;
			
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
			
			StateMachineValidator.Validate(this);
		}

		public void Initialize(GameObject gameObject, StateMachine stateMachine, ISharedData sharedData)
		{
			if (!gameObject || stateMachine == null) return;

			foreach (var stateNode in Nodes)
			{
				if (stateNode == null || !stateNode.State) continue;

				var instantiatedState = Instantiate(stateNode.State);
				if (!instantiatedState) continue;
				
				instantiatedState.GameObject = gameObject;
				instantiatedState.StateMachine = stateMachine;
				instantiatedState.SharedData = sharedData;
				stateNode.State = instantiatedState;
				stateNode.Awake();
			}
		}

		public bool TryGetNodeByState<T>(out StateNodeModel stateNodeModel) where T : State
		{
			stateNodeModel = Nodes.FirstOrDefault(node => node is { State: T });
			return stateNodeModel != null;
		}
		
		public bool TryGetNodeById(string id, out StateNodeModel stateNodeModel)
		{
			stateNodeModel = Nodes.FirstOrDefault(node => node.Id.Equals(id));
			return stateNodeModel != null;
		}

		public void SetParent(StateMachineModel stateMachineModel)
		{
			if (stateMachineModel == this) return;
			
			Parent = stateMachineModel;
		}
	}
}