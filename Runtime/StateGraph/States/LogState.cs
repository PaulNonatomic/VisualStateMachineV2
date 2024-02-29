using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Blue), NodeIcon(NodeIcon.V2_Note)]
	public class LogState : State
	{
		[Transition]
		public event Action OnExit;
		
		[SerializeField, Multiline(3)] 
		private string _message = "Hello World";
		
		public override void OnEnterState()
		{
			Debug.Log(_message);
			OnExit?.Invoke();
		}

		public override void OnExitState()
		{
			//Do nothing
		}
	}
}