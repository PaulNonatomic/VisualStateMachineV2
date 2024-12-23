using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.Validation
{
	public static class StateNodeValidator
	{
		public static void InitializePorts(StateNodeModel node)
		{
			// Ensure InputPorts and OutputPorts lists are initialized
			if (node.InputPorts == null)
			{
				node.InputPorts = new List<PortModel>();
			}

			if (node.OutputPorts == null)
			{
				node.OutputPorts = new List<PortModel>();
			}

			// Perform validation, which will create the necessary ports
			ValidateInputPorts(node);
			ValidateOutputPorts(node);
		}

		public static bool ValidateInputPorts(StateNodeModel node)
		{
			var changesMade = false;
			var type = node.State.GetType();

			// Retrieve methods with the EnterAttribute and zero or one parameter
			var enterMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null && !m.IsAbstract)
				.Where(m => m.GetParameters().Length <= 1) // Include methods with 0 or 1 parameter
				.ToList();

			// Synchronize input ports with the methods
			changesMade |= SynchronizePorts(
				node,
				node.InputPorts,
				enterMethods,
				m => GetUniqueMethodName(m), // Use unique method names as port IDs
				m => GetMethodParameterType(m),
				(n, index, member) => AddInputPort(n, index, member),
				"Method");

			// Log unsupported methods (more than one parameter)
			var unsupportedMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null && !m.IsAbstract)
				.Where(m => m.GetParameters().Length > 1) // Methods with more than one parameter
				.ToList();

			foreach (var method in unsupportedMethods)
			{
				GraphLog.LogWarning($"Method '{method.Name}' has more than one parameter and will be ignored.");
			}

			return changesMade;
		}

		private static Type GetMethodParameterType(MethodInfo method)
		{
			var parameters = method.GetParameters();

			// If the method has exactly one parameter, return its type
			if (parameters.Length == 1)
			{
				return parameters[0].ParameterType;
			}

			// For methods with zero parameters, return null
			if (parameters.Length == 0)
			{
				return null;
			}

			// Methods with more than one parameter are not supported
			return null;
		}

		public static bool ValidateOutputPorts(StateNodeModel node)
		{
			var changesMade = false;
			var type = node.State.GetType();

			// Retrieve events with the TransitionAttribute, sorted by unique name
			var transitionEvents = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(e => e.GetCustomAttribute<TransitionAttribute>() != null)
				.ToArray();

			// Synchronize output ports with the events
			changesMade |= SynchronizePorts(
				node,
				node.OutputPorts,
				transitionEvents,
				e => e.Name, // Assuming event names are unique
				e => GetEventParameterType(e),
				(n, index, member) => AddOutputPort(n, index, member),
				"Event");

			return changesMade;
		}

		private static bool SynchronizePorts<TMemberInfo>(
			StateNodeModel node,
			List<PortModel> portDatas,
			IEnumerable<TMemberInfo> members,
			Func<TMemberInfo, string> getMemberName,
			Func<TMemberInfo, Type> getMemberParameterType,
			Action<StateNodeModel, int, TMemberInfo> addPort,
			string memberTypeName)
		{
			var changesMade = false;

			// Build a dictionary of members by unique port ID
			var memberInfoDict = members.ToDictionary(getMemberName, m => m);

			// Remove ports that no longer have corresponding members
			for (var i = portDatas.Count - 1; i >= 0; i--)
			{
				var portData = portDatas[i];
				var portId = portData.Id;

				if (!memberInfoDict.ContainsKey(portId))
				{
					// PortData ID does not match any member name
					GraphLog.LogWarning($"Removing PortData with ID '{portId}' as it does not match any {memberTypeName}.");
					portDatas.RemoveAt(i);
					changesMade = true;
					continue;
				}

				// Update port type if necessary
				var member = memberInfoDict[portId];
				var expectedType = getMemberParameterType(member);
				var expectedPortTypeName = expectedType?.FullName ?? string.Empty;
				var currentPortTypeName = portData.PortTypeName ?? string.Empty;

				if (!string.Equals(currentPortTypeName, expectedPortTypeName, StringComparison.Ordinal))
				{
					// If both are empty strings (null or no type), consider them equal
					if (string.IsNullOrEmpty(currentPortTypeName) && string.IsNullOrEmpty(expectedPortTypeName))
					{
						// Types match (both are null or empty), no action needed
					}
					else
					{
						GraphLog.LogWarning($"PortTypeName mismatch for PortData '{portId}'. Expected '{expectedPortTypeName}', found '{currentPortTypeName}'. Updating port type.");
						portData.SetPortType(expectedType);
						changesMade = true;
					}
				}

				// Remove the member from the dictionary to track processed ports
				memberInfoDict.Remove(portId);
			}

			// Add missing PortData items with unique indices
			foreach (var kvp in memberInfoDict)
			{
				var portId = kvp.Key;
				var member = kvp.Value;

				// Assign a unique index (e.g., max existing index + 1)
				var newIndex = portDatas.Count > 0 ? portDatas.Max(pd => pd.Index) + 1 : 0;

				GraphLog.LogWarning($"Adding missing PortData for {memberTypeName} '{portId}' at index {newIndex}.");
				addPort(node, newIndex, member);
				changesMade = true;
			}

			return changesMade;
		}

		private static void AddInputPort(StateNodeModel node, int index, MethodInfo method)
		{
			var attribute = method.GetCustomAttribute<EnterAttribute>();
			var portModel = attribute?.GetPortData(method, index);

			if (portModel == null)
			{
				GraphLog.LogWarning($"Failed to get PortData for method '{method.Name}'.");
				return;
			}

			// Generate unique port ID based on method name and parameter type
			var uniquePortId = GetUniqueMethodName(method);
			portModel.Id = uniquePortId; // Ensure port ID is unique

			// Set the port type based on method parameters
			var parameters = method.GetParameters();
			if (parameters.Length == 1)
			{
				portModel.SetPortType(parameters[0].ParameterType);
			}
			else if (parameters.Length == 0)
			{
				// For methods with zero parameters, set port type to null
				portModel.SetPortType(null);
			}
			else
			{
				// Methods with more than one parameter are not supported
				// Should not reach here due to earlier filtering, but added for safety
				GraphLog.LogWarning($"Method '{method.Name}' has more than one parameter and will not have a port.");
				return;
			}

			node.InputPorts.Add(portModel);
		}

		private static void AddOutputPort(StateNodeModel node, int index, EventInfo eventInfo)
		{
			var attribute = eventInfo.GetCustomAttribute<TransitionAttribute>();
			var portModel = attribute.GetPortData(eventInfo, index);

			if (portModel == null)
			{
				GraphLog.LogWarning($"Failed to get PortData for event '{eventInfo.Name}'.");
				return;
			}

			// Set the port type based on event parameters
			var parameterType = GetEventParameterType(eventInfo);
			portModel.SetPortType(parameterType);

			node.OutputPorts.Add(portModel);
		}

		private static Type GetEventParameterType(EventInfo eventInfo)
		{
			var handlerType = eventInfo.EventHandlerType;
			var invokeMethod = handlerType.GetMethod("Invoke");
			var parameters = invokeMethod.GetParameters();

			if (parameters.Length == 1)
			{
				return parameters[0].ParameterType;
			}
			else
			{
				// Events with zero or multiple parameters are not supported
				return null;
			}
		}

		private static string GetUniqueMethodName(MethodInfo method)
		{
			var parameters = method.GetParameters();
			if (parameters.Length == 0)
			{
				return method.Name;
			}
			else if (parameters.Length == 1)
			{
				return $"{method.Name}_{parameters[0].ParameterType.Name}";
			}
			else
			{
				// Not supported
				return method.Name;
			}
		}
	}
}
