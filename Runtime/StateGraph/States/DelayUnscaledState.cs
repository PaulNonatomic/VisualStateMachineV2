﻿using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeWidth(width:190)]
	[NodeColor(NodeColor.Pink), NodeIcon(NodeIcon.Clock)]
	public class DelayUnscaledState : BaseDelayState
	{
		[Transition]
		public event Action OnComplete;
		
		[NonSerialized]
		private float _elapsedTime;

		public override void OnEnterState()
		{
			_elapsedTime = 0f;
		}
		
		public override void OnUpdateState()
		{
			_elapsedTime += Time.unscaledTime;
			
			if (_elapsedTime >= Duration)
			{
				OnComplete?.Invoke();
			}
		}

		public override void OnExitState()
		{
			
		}
	}
}