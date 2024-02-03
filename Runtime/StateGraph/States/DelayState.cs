using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Pink), NodeIcon(NodeIcon.V2_Clock, opacity:0.8f)]
	public class DelayState : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField, Tooltip("Duration in seconds")] 
		private float _duration = 1f;
		
		[SerializeField] 
		private bool _useScaledTime = true;
		
		[NonSerialized]
		private float _elapsedTime;

		public override void Enter()
		{
			_elapsedTime = 0f;
		}
		
		public override void Update()
		{
			_elapsedTime += _useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
			
			if (_elapsedTime >= _duration)
			{
				OnComplete?.Invoke();
			}
		}

		public override void Exit()
		{
			
		}
	}
}