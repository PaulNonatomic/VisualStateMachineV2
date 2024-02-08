using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeWidth(width:190), NodeColor(NodeColor.Teal), NodeIcon(NodeIcon.V2_Clock)]
	public class DelayState : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField, Tooltip("Duration in seconds")] 
		public float Duration = 1f;
		
		[NonSerialized]
		private float _elapsedTime;

		public override void Enter()
		{
			_elapsedTime = 0f;
		}
		
		public override void Update()
		{
			_elapsedTime += Time.deltaTime;
			
			if (_elapsedTime >= Duration)
			{
				OnComplete?.Invoke();
			}
		}

		public override void Exit()
		{
			
		}
	}
}