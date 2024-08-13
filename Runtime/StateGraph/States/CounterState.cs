using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	/**
	 * Remember that States~ are derived from ScriptableObject so
	 * it's important to flag that the Count value should not be
	 * serialized, although we reset the value on awake any way.
	 */
	[NodeWidth(width:190), NodeColor(NodeColor.Dijon), NodeIcon(NodeIcon.PlusCircle)]
	public class CounterState : BaseCounterState
	{
		[Transition(frameDelay:0)] public event Action OnComplete;
	
		public override void OnEnterState()
		{
			IncrementCounter();
			OnComplete?.Invoke();
		}

		public override void OnExitState()
		{
			//...
		}
	}
}