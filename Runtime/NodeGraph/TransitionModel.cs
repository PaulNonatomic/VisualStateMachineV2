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
		
		public void Transition()
		{
			OnTransition?.Invoke(this);
		}
	}
}