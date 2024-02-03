namespace Nonatomic.VSM2.StateGraph.States
{
	public abstract class JumpState : State
	{
		public JumpId JumpId;

		public override void Enter()
		{
			this.StateMachine.JumpTo(this.JumpId);
		}

		public override void Exit()
		{
		}
	}
}