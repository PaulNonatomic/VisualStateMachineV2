using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.LightBlue), NodeIcon(NodeIcon.V2_Random)]
	public class RandomTwoState : State
	{
		[Transition] public event Action OnOutcomeA;
		[Transition] public event Action OnOutcomeB;
		
		private Random _random = new();

		public override void OnEnterState()
		{
			switch (_random.Next(0, 1))
			{
				case 0:
					OnOutcomeA?.Invoke();
					break;
				case 1:
					OnOutcomeB?.Invoke();
					break;
			}
		}

		public override void OnExitState()
		{
			//...
		}
	}
}