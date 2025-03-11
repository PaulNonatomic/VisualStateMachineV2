using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeWidth(width:190), NodeColor(NodeColor.Teal), NodeIcon(NodeIcon.Clock)]
	public class DelayState : BaseDelayState
	{
		[Transition(frameDelay:0)]
		public event Action OnComplete;
		
		[NonSerialized]
		private float _elapsedTime;

		[Enter]
		public override void OnEnter()
		{
			Debug.Log($"DelayState.OnEnter");
			_elapsedTime = 0f;
		}
		
		public override void OnUpdate()
		{
			_elapsedTime += Time.deltaTime;
			Debug.Log($"DelayState.OnUpdate: {_elapsedTime}, {Duration}");
			if (_elapsedTime < Duration) return;
			OnComplete?.Invoke();
		}

		public override void OnExit()
		{
			Debug.Log($"DelayState.OnExit");
			//...
		}
	}
}