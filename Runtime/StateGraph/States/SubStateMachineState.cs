using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class SubStateMachineState : BaseSubStateMachineState
	{
		[Transition(frameDelay:0)]
		public event Action OnComplete;
		
		protected override void OnSubStateComplete(State state, StateMachineModel model)
		{
			base.OnSubStateComplete(state, model);
			OnComplete?.Invoke();
		}
	}
}