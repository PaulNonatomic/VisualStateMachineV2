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
		/// The OnEnter method is the entry point for each state
		/// States can require parameters by overloading the OnEnterState method
		/// </summary>
		public virtual void OnEnterState()
		{
			//...
		}

		public virtual void OnExitState()
		{
			//...
		}
		
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
		
		// Method to get all supported parameter types for OnEnterState methods
		public IEnumerable<Type> GetSupportedParameterTypes()
		{
			return GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
				.SelectMany(m => m.GetParameters())
				.Select(p => p.ParameterType)
				.Distinct();
		}
	}
}