using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class JumpOutState : JumpState
	{
		public override void Enter()
		{
			StateMachine.JumpTo(this.JumpId);
		}
	}
}