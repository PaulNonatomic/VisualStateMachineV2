namespace Nonatomic.VSM2.StateGraph.States
{
	public abstract class JumpState : State
	{
		public JumpId JumpId;

		[Enter]
		public override void OnEnter()
		{
			this.StateMachine.JumpTo(this.JumpId);
		}

		public override void OnExit()
		{
		}
	}
}