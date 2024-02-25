using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class JumpOutState : JumpState
	{
		public override void OnEnterState()
		{
			StateMachine.JumpTo(this.JumpId);
		}
	}
}