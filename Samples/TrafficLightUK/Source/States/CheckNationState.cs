using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;

namespace Samples.TrafficLightUK.States
{
	public enum Nation
	{
		UK,
		US
	}
	
	public class CheckNationState : State
	{
		[Transition] 
		public event System.Action OnUK;
		
		[Transition]
		public event System.Action OnUS;
		
		[SerializeField] private Nation _nation;
		
		public override void Enter()
		{
			switch (_nation)
			{
				case Nation.UK:
					OnUK?.Invoke();
					break;
				case Nation.US:
					OnUS?.Invoke();
					break;
			}
		}

		public override void Exit()
		{
			//...
		}
	}
}