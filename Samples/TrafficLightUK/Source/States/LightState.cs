using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;

namespace Samples.TrafficLightUK.States
{
	public class LightState : State
	{
		[Transition]
		public event Action OnComplete;

		[SerializeField, Multiline(4)] 
		private string _description;
		
		[SerializeField] 
		private bool _redLight;
		
		[SerializeField] 
		private bool _amberLight;
		
		[SerializeField] 
		private bool _greenLight;
		
		public override void Enter()
		{
			var trafficLight = GameObject.GetComponent<TrafficLightController>();
			trafficLight.ToggleLights(_redLight, _amberLight, _greenLight);
			
			OnComplete?.Invoke();
		}
		
		public override void Exit()
		{
			//...
		}
	}
}