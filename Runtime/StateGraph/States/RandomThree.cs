﻿using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class RandomThree : State
	{
		[Transition] public event Action OnOutcomeA;
		[Transition] public event Action OnOutcomeB;
		[Transition] public event Action OnOutcomeC;
		
		private Random _random = new();

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