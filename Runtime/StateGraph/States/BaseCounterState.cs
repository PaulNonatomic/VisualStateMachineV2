using System;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	public abstract class BaseCounterState : State
	{
		[NonSerialized, Tooltip("Counter")] 
		public int Count = 0;
		
		public override void OnAwakeState()
		{
			Count = 0;
		}
		
		protected virtual void IncrementCounter()
		{
			Count++;
		}
	}
}