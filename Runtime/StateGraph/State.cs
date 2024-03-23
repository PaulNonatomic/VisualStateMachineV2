#pragma warning disable 0067

using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	[NodeColor(NodeColor.Orange), NodeIcon(NodeIcon.Cube)]
	public abstract class State : ScriptableObject
	{
		public GameObject GameObject { get; set; }
		public StateMachine StateMachine { get; set; }
		
		public abstract void OnEnterState();
		public abstract void OnExitState();
		
		/**
		 * Unity life cycle methods are optional to override.
		 */
		
		public virtual void OnAwakeState()
		{
			//..
		}
		
		public virtual void OnStartState()
		{
			//..
		}

		public virtual void OnUpdateState()
		{
			//..
		}

		public virtual void OnFixedUpdateState()
		{
			//..
		}

		public virtual void OnDestroyState()
		{
			//..
		}
	}
}