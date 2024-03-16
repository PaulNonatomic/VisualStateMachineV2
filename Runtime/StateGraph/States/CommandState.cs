using System;
using System.Collections.Generic;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.StateGraph.Commands;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.LightBlue), NodeIcon(NodeIcon.V2_Command)]
	public class CommandState : State
	{
		[Transition] public event Action OnComplete;
		
		[SerializeField] private List<ScriptableCommand> _commands;
		
		public override void OnEnterState()
		{
			foreach(var cmd in _commands) cmd?.Execute();
			
			OnComplete?.Invoke();
		}

		public override void OnExitState()
		{
			//...
		}
	}
}