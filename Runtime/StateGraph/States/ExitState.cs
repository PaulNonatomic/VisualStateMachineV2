using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeIcon(NodeIcon.V2_Exit, opacity:0.8f)]
	[NodeColor(NodeColor.Red)]
	public class ExitState : State
	{
		public override void Enter()
		{
			StateMachine.Complete(this);
		}

		public override void Exit()
		{
			
		}
	}
}