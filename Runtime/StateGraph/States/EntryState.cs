using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeIcon(NodeIcon.Enter)]
	[HideInStateSelector, NodeColor(NodeColor.Green)]
	public class EntryState : State
	{
		[Transition(portColor:NodeColor.Green, frameDelay:1)] 
		public event Action OnEntry;

		public override void OnEnterState()
		{
			OnEntry?.Invoke();
		}

		public override void OnExitState()
		{
			//..
		}
	}
}