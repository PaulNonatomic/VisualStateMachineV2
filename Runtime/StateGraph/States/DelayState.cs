using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeWidth(width:190)]
	[NodeColor(NodeColor.Pink), NodeIcon(NodeIcon.V2_Clock)]
	public class DelayState : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] 
		private bool _unscaledTime = false;
		
		[SerializeField, Tooltip("Duration in seconds")] 
		private float _duration = 1f;
		
		[NonSerialized]
		private float _elapsedTime;

		public override void Enter()
		{
			_elapsedTime = 0f;
		}
		
		public override void Update()
		{
			_elapsedTime += _unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
			
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