using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class JumpInState : JumpState
	{
		[Transition(frameDelay:0)] 
		public event Action OnExit;
		
		[Enter]
		public override void OnEnterState()
		{
			OnExit?.Invoke();
		}

		public override void OnExitState()
		{
			
		}
	}
}