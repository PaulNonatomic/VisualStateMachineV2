﻿using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeIcon(NodeIcon.V2_Enter)]
	[HideInStateSelector, NodeColor(NodeColor.Green)]
	public class EntryState : State
	{
		[Transition(portColor:NodeColor.Green)] 
		public event Action OnEntry;

		public override void Enter()
		{
			OnEntry?.Invoke();
		}

		public override void Exit()
		{
			//..
		}
	}
}