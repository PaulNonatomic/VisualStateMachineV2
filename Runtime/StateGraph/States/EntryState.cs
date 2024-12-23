using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeIcon(NodeIcon.Enter)]
	[HideInStateSelector, NodeColor(NodeColor.Green)]
	public class EntryState : State
	{
		[Transition(portColor:NodeColor.Green, frameDelay:0)] 
		public event Action OnEntry;

		[Enter]
		public override void OnEnter()
		{
			OnEntry?.Invoke();
		}

		public override void OnExit()
		{
			//..
		}
	}
}