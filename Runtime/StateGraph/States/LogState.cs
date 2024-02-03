using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Blue), NodeIcon(NodeIcon.V2_Note, opacity:0.8f)]
	public class LogState : State
	{
		[Transition]
		public event Action OnExit;
		
		[SerializeField, Multiline(3)] 
		private string _message = "Hello World";
		
		public override void Enter()
		{
			Debug.Log(_message);
			OnExit?.Invoke();
		}

		public override void Exit()
		{
			//Do nothing
		}
	}
}