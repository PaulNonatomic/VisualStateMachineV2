using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class JumpOutState : JumpState
	{
		[Enter]
		public override void OnEnterState()
		{
			StateMachine.JumpTo(this.JumpId);
		}
	}
}