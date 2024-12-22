#pragma warning disable 0067

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Data;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	[NodeColor(NodeColor.Orange), NodeIcon(NodeIcon.Cube)]
	public abstract class State : ScriptableObject
	{
		public GameObject GameObject { get; set; }
		public StateMachine StateMachine { get; set; }
		public ISharedData SharedData { get; set; }
		public TransitionEventData TransitionData { get; set; }

		/// <summary>
		/// The OnStart method is mapped the state machines Awake method
		/// </summary>
		public virtual void OnAwake()
		{
			//...
		}
		
		/// <summary>
		/// The OnStart method is mapped the state machines Start method
		/// </summary>
		public virtual void OnStart()
		{
			//...
		}
		
		/// <summary>
		/// The OnEnter method is the entry point for each state
		/// States can require parameters by overloading the OnEnter method
		/// and dressing it with the [Enter] attribute
		/// </summary>
		public virtual void OnEnter()
		{
			//...
		}
		
		/// <summary>
		/// The OnUpdate method is mapped the state machines Update loop
		/// </summary>
		public virtual void OnUpdate()
		{
			//..
		}

		/// <summary>
		/// The OnFixedUpdate method is mapped the state machines FixedUpdate loop
		/// </summary>
		public virtual void OnFixedUpdate()
		{
			//..
		}

		/// <summary>
		/// The OnLateUpdate method is mapped to the state machine's LateUpdate loop.
		/// </summary>
		public virtual void OnLateUpdate()
		{
			//..
		}

		/// <summary>
		/// The OnExit method is the exit point for each state
		/// Useful for clean up and resetting values
		/// </summary>
		public virtual void OnExit()
		{
			//...
		}
		
		/// <summary>
		/// The OnDestroy method is called when the State Machine is destroyed
		/// Useful for clean up
		/// </summary>
		public virtual void OnDestroy()
		{
			//..
		} 
		
		// Method to get all supported parameter types for OnEnter methods
		public IEnumerable<Type> GetSupportedParameterTypes()
		{
			return GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
				.SelectMany(m => m.GetParameters())
				.Select(p => p.ParameterType)
				.Distinct();
		}

		/// <summary>
		/// Do NOT use Unity life cycle methods
		/// Instead use OnEnter
		/// </summary>
		[Obsolete("Do not use")]
		protected void OnEnable() { }

		/// <summary>
		/// Do NOT use Unity life cycle methods
		/// Instead use OnExit
		/// </summary>
		[Obsolete("Do not use")]
		protected void OnDisable() { }
		
		/// <summary>
		/// Do NOT use Unity life cycle methods
		/// Instead use OnAwake
		/// </summary>
		[Obsolete("Use OnAwake instead")]
		protected void Awake() { }
		
		/// <summary>
		/// Do NOT use Unity life cycle methods
		/// Instead use OnStart
		/// </summary>
		[Obsolete("Use OnStart instead")]
		protected void Start() { }
		
		
		//Obsolete code for migration
		[Obsolete("Use OnAwake instead")]
		public virtual void OnAwakeState()
		{
			OnAwake();
		}
		
		[Obsolete("Use OnStart instead")]
		public virtual void OnStartState()
		{
			OnStart();
		}

		[Obsolete("Use OnUpdate instead")]
		public virtual void OnUpdateState()
		{
			OnUpdate();
		}

		[Obsolete("Use OnFixedUpdate instead")]
		public virtual void OnFixedUpdateState()
		{
			OnFixedUpdate();
		}

		[Obsolete("Use OnDestroy instead")]
		public virtual void OnDestroyState()
		{
			OnDestroy();
		}
	}
}