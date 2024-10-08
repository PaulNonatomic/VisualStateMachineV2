﻿using System;
using System.Collections.Generic;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph
{
	[Serializable]
	public class StateTransitionModel : TransitionModel, IEqualityComparer<TransitionModel>
	{
		public event Action OnTransitionBegin;
		public event Action OnTransitionUpdate;
		public event Action OnTransitionEnd;
		
		public StateTransitionModel(string originNodeId, PortModel originPort, string destinationNodeId, PortModel destinationPort)
		{
			OriginNodeId = originNodeId;
			OriginPort = originPort;
			DestinationNodeId = destinationNodeId;
			DestinationPort = destinationPort;
		}

		public void Start()
		{
			OnTransitionBegin?.Invoke();
		}
		
		public void Update()
		{
			OnTransitionUpdate?.Invoke();
		}

		public void End()
		{
			OnTransitionEnd?.Invoke();
		}

		public bool Equals(TransitionModel x, TransitionModel y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;
			
			return x.OriginNodeId == y.OriginNodeId && 
				   x.DestinationNodeId == y.DestinationNodeId && 
				   Equals(x.OriginPort, y.OriginPort) && 
				   Equals(x.DestinationPort, y.DestinationPort);
		}

		public int GetHashCode(TransitionModel obj)
		{
			return HashCode.Combine(obj.OriginNodeId, obj.DestinationNodeId, obj.OriginPort, obj.DestinationPort);
		}
		
		public StateTransitionModel Clone()
		{
			var clone = new StateTransitionModel(this.OriginNodeId, this.OriginPort, this.DestinationNodeId, this.DestinationPort);
			return clone;
		}
	}
}