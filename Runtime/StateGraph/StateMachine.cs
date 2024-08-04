using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	public class StateMachine
	{
		public event Action<State, StateMachineModel> OnComplete;
		
		public StateMachineModel Model { get; private set; }
		public bool IsComplete { get; private set; }
		
		private StateNodeModel _currentNode;
		private Dictionary<string, StateNodeModel> _nodeLookup = new();
		private Dictionary<string, List<StateTransitionModel>> _transitionLookup = new();
		private Dictionary<JumpId, StateNodeModel> _jumpNodeLookup = new();
		private CancellationTokenSource _cancellationTokenSource = new();
		
		public StateMachine(StateMachineModel model, GameObject gameObject)
		{
			Model = StateMachineModel.CreateInstance(model);
			Model.Initialize(gameObject, this);

			CreateNodeLookupTable();
			CreateTransitionLookupTable();
		}

		public void Update()
		{
			if (_currentNode == null) return;
			if (!_currentNode.Active) return;
			
			_currentNode.Update();
		}

		public void FixedUpdate()
		{
			if (_currentNode == null) return;
			if (!_currentNode.Active) return;
			
			_currentNode.FixedUpdate();
		}

		public void Start()
		{
			foreach (var node in Model.Nodes)
			{
				if(!node.Enabled) continue;
				node.Start();
			}
		}

		public void Enter()
		{
			if (Model.TryGetNodeByState<EntryState>(out var entryNode))
			{
				_currentNode = entryNode;
			}
			
			IsComplete = false;
			SubscribeToNode(_currentNode);

			_currentNode.Enter();
		}

		public void Exit()
		{
			if (_currentNode == null) return;
			
			UnsubscribeFromNode(_currentNode);
			_currentNode.Exit();
			IsComplete = true;
		}
		
		public void JumpTo(JumpId jumpId)
		{
			if (!_jumpNodeLookup.TryGetValue(jumpId, out var nextNode))
			{
				throw new Exception($"There is no JumpIn state with the id:{jumpId} in {this.Model.name}");
			}

			if (nextNode == null) return;

			TriggerAsyncTransition(nextNode);
		}

		public void OnDestroy()
		{
			_cancellationTokenSource.Cancel();

			if (_currentNode != null)
			{
				UnsubscribeFromNode(_currentNode);
				_currentNode.Exit();
			}
			
			foreach(var kvp in _nodeLookup)
			{
				var nodeId = kvp.Key;
				var node = _nodeLookup[nodeId];
				node?.OnDestroy();
			}

			Model = null;
		}

		private void CreateTransitionLookupTable()
		{
			_transitionLookup.Clear();

			foreach (var transition in Model.Transitions)
			{
				if (!_transitionLookup.ContainsKey(transition.OriginNodeId))
				{
					_transitionLookup.Add(transition.OriginNodeId, new List<StateTransitionModel>());
				}
				
				_transitionLookup[transition.OriginNodeId].Add(transition);
			}
		}
		
		private void CreateNodeLookupTable()
		{
			_nodeLookup.Clear();
			_jumpNodeLookup.Clear();

			foreach (var node in Model.Nodes)
			{
				if (!node.State) continue;
				_nodeLookup.Add(node.Id, node);

				if (node.State is not JumpInState jumpIn) continue;
				_jumpNodeLookup.Add(jumpIn.JumpId, node);
			}
		}

		private void SubscribeToNode(StateNodeModel node)
		{
			if (node == null)
			{
				throw new NullReferenceException("SubStateMachine cannot subscribe to null node");
			}

			if (!_transitionLookup.TryGetValue(node.Id, out var transitions)) return;

			foreach (var transition in transitions)
			{
				ToggleEventSubscriptionByName(node.State, transition, true);
			}
		}
		
		private void ToggleEventSubscriptionByName(object targetObject, StateTransitionModel transition, bool subscribe)
		{
			var eventName = transition.OriginPort.Id;
			var eventInfo = targetObject.GetType().GetEvent(eventName);
			if (eventInfo == null) return;
			
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, transition, nameof(transition.Transition));

			if (subscribe)
			{
				eventInfo.AddEventHandler(targetObject, handler);
				transition.OnTransition += HandleTransition;
			}
			else
			{
				eventInfo.RemoveEventHandler(targetObject, handler);
				transition.OnTransition -= HandleTransition;
			}
		}

		private void UnsubscribeFromNode(StateNodeModel node)
		{
			if (node == null)
			{
				throw new NullReferenceException("SubStateMachine cannot unsubscribe to null node");
			}
			
			if (!_transitionLookup.TryGetValue(node.Id, out var transitions)) return;

			foreach (var transition in transitions)
			{
				ToggleEventSubscriptionByName(node.State, transition, false);
			}
		}

		private void HandleTransition(TransitionModel transition)
		{
			var nextNode = _nodeLookup[transition.DestinationNodeId];
			TriggerAsyncTransition(nextNode, transition.OriginPort.FrameDelay);
		}

		private async void TriggerAsyncTransition(StateNodeModel nextNode, int frameDelay = 0)
		{
			try
			{
				await TransitionAsync(nextNode, frameDelay).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		private async Task TransitionAsync(StateNodeModel nextNode, int frameDelay = 0)
		{
			try
			{
				UnsubscribeFromNode(_currentNode);
				_currentNode.Exit();
				
				var transition = GetTransitionForCurrentNode(_currentNode, nextNode);
				transition?.TriggerTransition();
				
				if (!Application.isPlaying) return;

				for (var i = 0; i < frameDelay; i++)
				{
					if (_cancellationTokenSource.Token.IsCancellationRequested)
					{
						return;
					}
						
					await Task.Yield();
				}
				
				_currentNode = nextNode;
				if (_currentNode == null) return;
				
				SubscribeToNode(_currentNode);
				_currentNode.Enter();
			}
			catch (OperationCanceledException canceledException)
			{
				GraphLog.Log("SubStateMachine.TransitionAsync cancelled: " + canceledException.Message);
			}
		}

		public void Complete(State state)
		{
			if(IsComplete) return;

			IsComplete = true;
			OnComplete?.Invoke(state, Model);
		}

		public void SetParent(StateMachine stateMachine)
		{
			Model.SetParent(stateMachine.Model);
		}
		
		[CanBeNull]
		private StateTransitionModel GetTransitionForCurrentNode(StateNodeModel currentNode, StateNodeModel nextNode)
		{
			return _transitionLookup.TryGetValue(currentNode.Id, out var transitions) 
				? transitions.FirstOrDefault(t => t.DestinationNodeId == nextNode.Id) 
				: null;
		}
	}
}