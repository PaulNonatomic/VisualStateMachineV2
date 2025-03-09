using System;
using System.Collections.Generic;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Factory class responsible for creating node view instances based on state types
	/// </summary>
	public static class StateGraphNodeFactory
	{
		private static readonly Dictionary<Type, Type> _stateTypeToNodeViewType = new()
		{
			{ typeof(EntryState), typeof(EntryNodeView) },
			{ typeof(ExitState), typeof(ExitNodeView) },
			{ typeof(JumpInState), typeof(JumpNodeView) },
			{ typeof(JumpOutState), typeof(JumpNodeView) },
			{ typeof(DelayState), typeof(DelayNodeView) },
			{ typeof(RelayState), typeof(RelayNodeView) },
			{ typeof(CounterState), typeof(CounterNodeView) },
			{ typeof(CounterWithTargetState), typeof(CounterWithTargetNodeView) },
			{ typeof(DelayUnscaledState), typeof(DelayNodeView) },
			{ typeof(SubStateMachineState), typeof(SubStateNodeView) },
			{ typeof(StickyNoteState), typeof(StickyNoteNodeView) }
		};

		/// <summary>
		///     Creates a node view instance for a node model
		/// </summary>
		public static BaseStateNodeView MakeNode(GraphView graphView,
			StateNodeModel node,
			StateMachineModel model)
		{
			var stateType = node.State.GetType();
			var viewType = GetViewTypeByStateType(stateType);

			// Create the view without additional initialization
			var instance = (BaseStateNodeView)Activator.CreateInstance(viewType, graphView, model, node);
			return instance;
		}
		
		/// <summary>
		/// Creates a state node model of the specified type at the given position
		/// This is a facade method for backward compatibility
		/// </summary>
		public static StateNodeModel MakeStateNodeData(StateMachineModel model, 
			Type stateType, 
			Vector2 position)
		{
			return StateNodeModelFactory.CreateStateNodeModel(model, stateType, position);
		}
		
		/// <summary>
		/// Generates a unique name for a state based on its type
		/// This is a facade method for backward compatibility
		/// </summary>
		public static string GenerateStateName(Type stateType)
		{
			return StateNodeModelFactory.GenerateStateName(stateType);
		}

		/// <summary>
		///     Gets the appropriate view type for a state type
		/// </summary>
		private static Type GetViewTypeByStateType(Type stateType)
		{
			return _stateTypeToNodeViewType.TryGetValue(stateType, out var value)
				? value
				: typeof(StateNodeView);
		}
	}
}