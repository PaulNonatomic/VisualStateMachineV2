using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Blue), NodeIcon(NodeIcon.Note), NodeWidth(400)]
	public class LogState : State
	{
		[Transition]
		public event Action Exit;
		
		[SerializeField, Multiline(3)] 
		private string _message = "Hello World";
		
		[Enter]
		public override void OnEnter()
		{
			Debug.Log(_message);
			Exit?.Invoke();
		}

		public override void OnExit()
		{
			//...
		}
	}
}