using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class JumpOutState : JumpState
	{
		[Enter]
		public override void OnEnter()
		{
			StateMachine.JumpTo(this.JumpId);
		}
	}
}