#pragma warning disable 0067

using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	[NodeColor(NodeColor.Orange), NodeIcon(NodeIcon.V2_Cube)]
	public abstract class State : ScriptableObject
	{
		public GameObject GameObject { get; set; }
		public StateMachine StateMachine { get; set; }
		
		public abstract void Enter();
		public abstract void Exit();
		
		/**
		 * Unity life cycle methods are optional to override.
		 */
		
		public virtual void Awake()
		{
			//..
		}
		
		public virtual void Start()
		{
			//..
		}

		public virtual void Update()
		{
			//..
		}

		public virtual void FixedUpdate()
		{
			//..
		}

		public virtual void OnDestroy()
		{
			//..
		}
	}
}