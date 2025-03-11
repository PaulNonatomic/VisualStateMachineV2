using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	/// <summary>
	///     Manages transitions between states in the state machine
	/// </summary>
	public class StateTransitionManager
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private readonly EventSubscriptionService _eventSubscriptionService;
		private readonly Dictionary<JumpId, StateNodeModel> _jumpNodeLookup = new();
		private readonly Dictionary<string, StateNodeModel> _nodeLookup = new();
		private readonly Action<StateNodeModel> _onNodeChanged;
		private readonly Dictionary<string, List<StateTransitionModel>> _transitionLookup = new();

		private StateNodeModel _currentNode;
		private Action<TransitionModel, TransitionEventData> _onTransitionCallback;

		public StateTransitionManager(
			Dictionary<string, StateNodeModel> nodeLookup,
			Dictionary<string, List<StateTransitionModel>> transitionLookup,
			Dictionary<JumpId, StateNodeModel> jumpNodeLookup,
			EventSubscriptionService eventSubscriptionService,
			Action<StateNodeModel> onNodeChanged)
		{
			_nodeLookup = nodeLookup;
			_transitionLookup = transitionLookup;
			_jumpNodeLookup = jumpNodeLookup;
			_eventSubscriptionService = eventSubscriptionService;
			_onNodeChanged = onNodeChanged;
		}

		public void SetCurrentNode(StateNodeModel node)
		{
			_currentNode = node;
		}

		public StateNodeModel GetCurrentNode()
		{
			return _currentNode;
		}

		public void SetTransitionCallback(Action<TransitionModel, TransitionEventData> callback)
		{
			_onTransitionCallback = callback;
		}

		public void CancelAllTransitions()
		{
			_cancellationTokenSource.Cancel();
		}

		public void JumpTo(JumpId jumpId)
		{
			if (!_jumpNodeLookup.TryGetValue(jumpId, out var nextNode)) throw new Exception($"There is no JumpIn state with the id:{jumpId}");

			if (nextNode == null) return;

			TriggerAsyncTransition(nextNode, TransitionEventData.Empty);
		}

		public void HandleTransition(TransitionModel transition, TransitionEventData eventData)
		{
			var nextNode = _nodeLookup[transition.DestinationNodeId];
			_ = TriggerAsyncTransition(nextNode, eventData, transition.OriginPort.FrameDelay);
		}

		private async Task TriggerAsyncTransition(StateNodeModel nextNode, TransitionEventData eventData, int frameDelay = 0)
		{
			try
			{
				await TransitionAsync(nextNode, eventData, frameDelay).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private async Task TransitionAsync(StateNodeModel nextNode, TransitionEventData eventData, int frameDelay = 0)
		{
			try
			{
				_eventSubscriptionService.UnsubscribeFromNode(_currentNode);
				_currentNode.Exit();

				var stateTransitionModel = GetTransitionForCurrentNode(_currentNode, nextNode);
				stateTransitionModel?.Start();

				if (!Application.isPlaying) return;

				for (var i = 0; i < frameDelay; i++)
				{
					if (_cancellationTokenSource.Token.IsCancellationRequested)
					{
						stateTransitionModel?.End();
						return;
					}

					await Task.Yield();
					stateTransitionModel?.Update();
				}

				stateTransitionModel?.End();

				_currentNode = nextNode;

				// Notify about the node change
				_onNodeChanged?.Invoke(nextNode);

				if (_currentNode == null) return;

				_eventSubscriptionService.SubscribeToNode(_currentNode);
				_currentNode.Enter(eventData);
			}
			catch (OperationCanceledException canceledException)
			{
				GraphLog.Log("TransitionAsync cancelled: " + canceledException.Message);
			}
		}

		private StateTransitionModel GetTransitionForCurrentNode(StateNodeModel currentNode, StateNodeModel nextNode)
		{
			return _transitionLookup.TryGetValue(currentNode.Id, out var transitions)
				? transitions.FirstOrDefault(t => t.DestinationNodeId == nextNode.Id)
				: null;
		}
	}
}