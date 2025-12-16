using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Blue)]
	public class RelayState : State
	{
		[Transition(frameDelay:0)] public event Action OnComplete;
		
		[Enter]
		public override void OnEnter()
		{
			OnComplete?.Invoke();
		}

		public override void OnExit()
		{
			//...
		}
	}
}