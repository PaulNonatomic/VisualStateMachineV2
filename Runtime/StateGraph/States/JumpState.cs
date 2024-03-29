﻿namespace Nonatomic.VSM2.StateGraph.States
{
	public abstract class JumpState : State
	{
		public JumpId JumpId;

		public override void OnEnterState()
		{
			this.StateMachine.JumpTo(this.JumpId);
		}

		public override void OnExitState()
		{
		}
	}
}