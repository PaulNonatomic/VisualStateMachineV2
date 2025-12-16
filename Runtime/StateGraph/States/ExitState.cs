using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeIcon(NodeIcon.Exit)]
	[NodeColor(NodeColor.Red)]
	public class ExitState : State
	{
		[Enter]
		public override void OnEnter()
		{
			StateMachine.Complete(this);
		}

		public override void OnExit()
		{
			
		}
	}
}