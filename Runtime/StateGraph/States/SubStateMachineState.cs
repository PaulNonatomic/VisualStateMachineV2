using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class SubStateMachineState : BaseSubStateMachineState
	{
		[Transition]
		public event Action OnComplete;
		
		protected override void HandleComplete(State state)
		{
			OnComplete?.Invoke();
		}

		public override void Exit()
		{
			//...
		}
	}
}