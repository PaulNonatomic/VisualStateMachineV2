using System;
using UnityEngine;

namespace Nonatomic.VSM2.NodeGraph
{
	[Serializable]
	public class TransitionModel
	{
		public event Action<TransitionModel> OnTransition;
		
		public string OriginNodeId;
		public string DestinationNodeId;
		
		public PortModel OriginPort;
		public PortModel DestinationPort;
		
		private Delegate _onTransitionWithArg;
		
		public void Transition()
		{
			Debug.Log("Transition");
			OnTransition?.Invoke(this);
		}
		
		public void Transition<T>(T value)
		{
			Debug.Log($"Transition<T>:{value}");
			(_onTransitionWithArg as Action<TransitionModel, T>)?.Invoke(this, value);
		}
		
		private void AddTransitionHandler<T>(Action<TransitionModel, T> handler)
		{
			_onTransitionWithArg = Delegate.Combine(_onTransitionWithArg, handler);
		}

		private void RemoveTransitionHandler<T>(Action<TransitionModel, T> handler)
		{
			_onTransitionWithArg = Delegate.Remove(_onTransitionWithArg, handler);
		}
	}
}