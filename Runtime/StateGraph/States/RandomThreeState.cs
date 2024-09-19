using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.LightBlue), NodeIcon(NodeIcon.Random)]
	public class RandomThreeState : State
	{
		[Transition] public event Action OnOutcomeA;
		[Transition] public event Action OnOutcomeB;
		[Transition] public event Action OnOutcomeC;
		
		private Random _random = new();

		[Enter]
		public override void OnEnterState()
		{
			switch (_random.Next(0, 2))
			{
				case 0:
					OnOutcomeA?.Invoke();
					break;
				case 1:
					OnOutcomeB?.Invoke();
					break;
				case 2:
					OnOutcomeC?.Invoke();
					break;
			}
		}

		public override void OnExitState()
		{
			//...
		}
	}
}