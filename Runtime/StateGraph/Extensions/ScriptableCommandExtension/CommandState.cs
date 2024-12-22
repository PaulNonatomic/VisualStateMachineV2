#if SCRIPTABLE_COMMANDS

using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.LightBlue), NodeIcon(NodeIcon.Command)]
	public class CommandState : BaseCommandState
	{
		[Transition] public event Action OnComplete;
		
		[Enter]
		public override void OnEnter()
		{
			base.OnEnter();
		}

		protected override void HandleTaskCompletion()
		{
			OnComplete?.Invoke();
		}
	}
}
#endif