using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class JumpInState : JumpState
	{
		[Transition(frameDelay:0)] 
		public event Action Exit;
		
		[Enter]
		public override void OnEnter()
		{
			Exit?.Invoke();
		}

		public override void OnExit()
		{
			
		}
	}
}