using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeIcon(NodeIcon.Post), NodeWidth(200)]
	[HideInStateSelector, NodeColor(NodeColor.Cantera)]
	public class StickyNoteState : State
	{
		[SerializeField, Multiline(5)] 
		private string _note;
		
		[Enter]
		public override void OnEnter()
		{
		}

		public override void OnExit()
		{
		}
	}
}