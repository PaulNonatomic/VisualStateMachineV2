using System;
using System.Collections.Generic;
using Nonatomic.VSM2.NodeGraph;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	/// <summary>
	///     Manages event subscriptions for state transitions
	/// </summary>
	public class EventSubscriptionService
	{
		private readonly Action<TransitionModel, TransitionEventData> _onTransitionCallback;
		private readonly Dictionary<string, List<StateTransitionModel>> _transitionLookup = new();

		public EventSubscriptionService(
			Dictionary<string, List<StateTransitionModel>> transitionLookup,
			Action<TransitionModel, TransitionEventData> onTransitionCallback)
		{
			_transitionLookup = transitionLookup;
			_onTransitionCallback = onTransitionCallback;
		}

		public void SubscribeToNode(StateNodeModel node)
		{
			if (node == null) throw new NullReferenceException("Cannot subscribe to null node");

			if (!_transitionLookup.TryGetValue(node.Id, out var transitions)) return;

			foreach (var transition in transitions)
			{
				Debug.Log("SubscribeToNode: " + node.State.name);
				ToggleEventSubscriptionByName(node.State, transition, true);
			}
		}

		public void UnsubscribeFromNode(StateNodeModel node)
		{
			if (node == null) throw new NullReferenceException("Cannot unsubscribe from null node");

			if (!_transitionLookup.TryGetValue(node.Id, out var transitions)) return;

			foreach (var transition in transitions)
			{
				ToggleEventSubscriptionByName(node.State, transition, false);
			}
		}

		private void ToggleEventSubscriptionByName(object targetObject, StateTransitionModel transition, bool subscribe)
		{
			var eventName = transition.OriginPort.Id;
			var targetType = targetObject.GetType();

			// Get EventInfo from the cache
			var eventInfo = ReflectionCache.GetEventInfo(targetType, eventName);
			if (eventInfo == null) return;

			// Check if this is an Action<T> or Action
			if (eventInfo.EventHandlerType.IsGenericType
				&& eventInfo.EventHandlerType.GetGenericTypeDefinition() == typeof(Action<>))
			{
				var argumentType = eventInfo.EventHandlerType.GenericTypeArguments[0];

				// Grab the generic Transition<T> method off StateTransitionModel
				var method = ReflectionCache.GetGenericMethod(
					typeof(StateTransitionModel),
					"Transition",
					new[] { argumentType },
					1);

				if (method == null) return;

				var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, transition, method);

				if (subscribe)
				{
					eventInfo.AddEventHandler(targetObject, handler);
					Debug.Log($"transition.OnTransition 1 += {transition.OriginNodeId}=>{transition.DestinationNodeId}");
					transition.OnTransition += _onTransitionCallback;
				}
				else
				{
					Debug.Log($"transition.OnTransition 1 -= {transition.OriginNodeId}=>{transition.DestinationNodeId}");
					eventInfo.RemoveEventHandler(targetObject, handler);
					transition.OnTransition -= _onTransitionCallback;
				}
			}
			else if (eventInfo.EventHandlerType == typeof(Action))
			{
				var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, transition, "Transition");
				if (subscribe)
				{
					Debug.Log($"transition.OnTransition 2 += {transition.OriginNodeId}=>{transition.DestinationNodeId}");
					eventInfo.AddEventHandler(targetObject, handler);
					transition.OnTransition += _onTransitionCallback;
				}
				else
				{
					Debug.Log($"transition.OnTransition 2 -= {transition.OriginNodeId}=>{transition.DestinationNodeId}");
					eventInfo.RemoveEventHandler(targetObject, handler);
					transition.OnTransition -= _onTransitionCallback;
				}
			}
		}
	}
}