using System;
using System.Collections.Generic;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using Nonatomic.VSM2.StateGraph.Validation;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public static class StateGraphNodeFactory
	{
		private static readonly Dictionary<Type, Type> _stateTypeToNodeViewType = new ()
		{
			{typeof(EntryState), typeof(EntryNodeView)},
			{typeof(ExitState), typeof(ExitNodeView)},
			{typeof(JumpInState), typeof(JumpNodeView)},
			{typeof(JumpOutState), typeof(JumpNodeView)},
			{typeof(DelayState), typeof(DelayNodeView)},
			{typeof(RelayState), typeof(RelayNodeView)},
			{typeof(CounterState), typeof(CounterNodeView)},
			{typeof(CounterWithTargetState), typeof(CounterWithTargetNodeView)},
			{typeof(DelayUnscaledState), typeof(DelayNodeView)},
			{typeof(SubStateMachineState), typeof(SubStateNodeView)},
			{typeof(StickyNoteState), typeof(StickyNoteNodeView)}
		};
		
		public static BaseStateNodeView MakeNode(GraphView graphView,
												 StateNodeModel node,
												 StateMachineModel model)
		{
			var stateType = node.State.GetType();
			var viewType = GetViewTypeByStateType(stateType);
			var instance = (BaseStateNodeView)Activator.CreateInstance(viewType, graphView, model, node);
			return instance;
		}

		public static StateNodeModel MakeStateNodeData(StateMachineModel model, 
			Type stateType, 
			Vector2 position)
		{
			var state = ScriptableObject.CreateInstance(stateType) as State;
			if (!state) return null;

			state.name = GenerateStateName(stateType);
			var stateNode = new StateNodeModel(state, position);
			StateNodeValidator.InitializePorts(stateNode);
			
			model.AddState(stateNode);

			return stateNode;
		}

		public static string GenerateStateName(Type stateType)
		{
			return  $"{stateType.Name}-{GUID.Generate()}";
		}
		
		private static Type GetViewTypeByStateType(Type stateType)
		{
			return _stateTypeToNodeViewType.TryGetValue(stateType, out var value) 
				? value 
				: typeof(StateNodeView);
		}
	}
}