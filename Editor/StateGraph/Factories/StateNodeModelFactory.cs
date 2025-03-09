using System;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Validation;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Factory class responsible for creating state node models
	/// </summary>
	public static class StateNodeModelFactory
	{
		/// <summary>
		///     Creates a state node model of the specified type at the given position
		/// </summary>
		public static StateNodeModel CreateStateNodeModel(StateMachineModel model,
			Type stateType,
			Vector2 position)
		{
			// Create the state instance
			var state = CreateStateInstance(stateType);
			if (state == null) return null;

			// Create the node model
			var stateNode = new StateNodeModel(state, position);

			// Initialize ports
			StateNodeValidator.InitializePorts(stateNode);

			// Add to model
			model.AddState(stateNode);

			return stateNode;
		}

		/// <summary>
		///     Creates a state instance of the specified type
		/// </summary>
		private static State CreateStateInstance(Type stateType)
		{
			var state = ScriptableObject.CreateInstance(stateType) as State;
			if (state == null) return null;

			state.name = GenerateStateName(stateType);
			return state;
		}

		/// <summary>
		///     Generates a unique name for a state based on its type
		/// </summary>
		public static string GenerateStateName(Type stateType)
		{
			return $"{stateType.Name}-{GUID.Generate()}";
		}
	}
}