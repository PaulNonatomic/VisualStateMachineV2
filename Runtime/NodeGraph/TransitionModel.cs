using System;
using UnityEngine;

namespace Nonatomic.VSM2.NodeGraph
{
	[Serializable]
	public class TransitionModel
	{
		public event Action<TransitionModel, TransitionEventData> OnTransition;
		
		public string OriginNodeId;
		public string DestinationNodeId;
		
		public PortModel OriginPort;
		public PortModel DestinationPort;
		
		public void Transition()
		{
			OnTransition?.Invoke(this, TransitionEventData.Empty);
		}
		
		public void Transition<T>(T value)
		{
			var eventData = new TransitionEventData(value, typeof(T));
			OnTransition?.Invoke(this, eventData);
		}
	}
}