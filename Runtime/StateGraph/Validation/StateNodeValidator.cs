using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;

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
			
			// Check if the State type has the [IgnoreDefaultEntryPort] attribute
			bool ignoreDefaultEntryPort = type.GetCustomAttribute<IgnoreDefaultEntryPortAttribute>() != null;

			// Retrieve methods with the EnterAttribute
			var enterMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
				.ToList();
			
			// Retrieve the OnEnterState method from the current type
			var method = type.GetMethod("OnEnterState", BindingFlags.Public | BindingFlags.Instance);
			var baseMethod = method?.GetBaseDefinition();
			
			MethodInfo defaultEntryMethod = null;
			
			if (method != null && method != baseMethod && method.GetParameters().Length == 0 && method.DeclaringType != baseMethod.DeclaringType)
			{
				// The method is overridden somewhere in the inheritance chain and has no parameters
				defaultEntryMethod = method;
				enterMethods.Add(defaultEntryMethod);
			}

			var defaultEntryPort = node.InputPorts.FirstOrDefault(p => string.Equals(p.Id, "Enter", StringComparison.Ordinal));
			
			if (!ignoreDefaultEntryPort && defaultEntryMethod != null)
			{
				// Synchronize default entry port
				changesMade |= SynchronizeDefaultEntryPort(node, defaultEntryMethod, defaultEntryPort);
			}
			else
			{
				// If default entry port exists but should be ignored, remove it
				if (defaultEntryPort != null)
				{
					GraphLog.LogWarning($"Ignoring default entry port as per attribute on State '{type.Name}'. Removing port '{defaultEntryPort.Id}'.");
					node.InputPorts.Remove(defaultEntryPort);
					changesMade = true;
				}
			}

			// Synchronize input ports with the methods
			changesMade |= SynchronizePorts(
				node,
				node.InputPorts,
				enterMethods,
				m => m.Name,
				m => m.GetParameters().FirstOrDefault()?.ParameterType,
				(n, index, member) => AddInputPort(n, index, member),
				"Method");

			return changesMade;
		}

		public static bool ValidateOutputPorts(StateNodeModel node)
		{
			var changesMade = false;
			var type = node.State.GetType();

			// Retrieve events with the TransitionAttribute
			var transitionEvents = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Where(e => e.GetCustomAttribute<TransitionAttribute>() != null)
				.ToArray();

			// Synchronize output ports with the events
			changesMade |= SynchronizePorts(
				node,
				node.OutputPorts,
				transitionEvents,
				e => e.Name,
				e => GetEventParameterType(e),
				(n, index, member) => AddOutputPort(n, index, member),
				"Event");

			return changesMade;
		}

		private static bool SynchronizeDefaultEntryPort(StateNodeModel node, MethodInfo defaultEntryMethod, PortModel defaultEntryPort)
		{
			var changesMade = false;

			if (defaultEntryMethod == null)
			{
				// Default method doesn't exist, remove default port if it exists
				if (defaultEntryPort != null)
				{
					GraphLog.LogWarning($"Default input port found, but method is missing. Removing port '{defaultEntryPort.Id}'.");
					node.InputPorts.Remove(defaultEntryPort);
					changesMade = true;
				}
			}
			else
			{
				// Default method exists, ensure default port exists
				if (defaultEntryPort == null)
				{
					GraphLog.LogWarning($"Default entry method found, but input port is missing. Adding port '{defaultEntryMethod.Name}'.");
					node.InputPorts.Add(PortModel.MakeDefaultEntryPort(0));
					changesMade = true;
				}
				else if (defaultEntryPort.Index != 0)
				{
					// Correct the index if necessary
					GraphLog.LogWarning($"Correcting index of default input port '{defaultEntryPort.Id}' from {defaultEntryPort.Index} to 0.");
					defaultEntryPort.Index = 0;
					changesMade = true;
				}
			}

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

			// Build dictionaries for quick lookup
			var memberInfoNames = new HashSet<string>();
			var memberInfoIndices = new Dictionary<string, int>();
			var memberInfoDict = new Dictionary<string, TMemberInfo>();

			var index = 0;
			foreach (var member in members)
			{
				var name = getMemberName(member);
				memberInfoNames.Add(name);
				memberInfoIndices[name] = index;
				memberInfoDict[name] = member;
				index++;
			}

			// Track indices already used to ensure uniqueness
			var usedIndices = new HashSet<int>();

			// Synchronize existing PortData items
			for (var i = portDatas.Count - 1; i >= 0; i--)
			{
				var portData = portDatas[i];
				var portId = portData.Id;

				if (!memberInfoNames.Contains(portId))
				{
					// PortData ID does not match any member name
					GraphLog.LogWarning($"Removing PortData with ID '{portId}' as it does not match any {memberTypeName}.");
					portDatas.RemoveAt(i);
					changesMade = true;
					continue;
				}

				// Check and correct the index if necessary
				var correctIndex = memberInfoIndices[portId];
				if (portData.Index != correctIndex)
				{
					GraphLog.LogWarning($"Correcting index of PortData '{portId}' from {portData.Index} to {correctIndex}.");
					portData.Index = correctIndex;
					changesMade = true;
				}

				// Ensure uniqueness of indices
				if (usedIndices.Contains(portData.Index))
				{
					GraphLog.LogWarning($"Duplicate index found for PortData '{portId}'. Removing entry.");
					portDatas.RemoveAt(i);
					changesMade = true;
					continue;
				}

				usedIndices.Add(portData.Index);

				// Update port type if necessary
				var expectedType = getMemberParameterType(memberInfoDict[portId]);
				var expectedPortTypeName = expectedType?.FullName ?? string.Empty;

				if (!string.Equals(portData.PortTypeName, expectedPortTypeName, StringComparison.Ordinal) &&
					!(string.IsNullOrEmpty(portData.PortTypeName) && string.IsNullOrEmpty(expectedPortTypeName)))
				{
					GraphLog.LogWarning($"PortTypeName mismatch for PortData '{portId}'. Expected '{expectedPortTypeName}', found '{portData.PortTypeName}'.");
					portData.SetPortType(expectedType);
					changesMade = true;
				}
			}

			// Add missing PortData items
			foreach (var member in members)
			{
				var name = getMemberName(member);
				if (portDatas.Any(pd => string.Equals(pd.Id, name, StringComparison.Ordinal))) continue;

				var newIndex = memberInfoIndices[name];
				if (usedIndices.Contains(newIndex)) continue;

				GraphLog.LogWarning($"Adding missing PortData for {memberTypeName} '{name}' at index {newIndex}.");
				addPort(node, newIndex, member);
				usedIndices.Add(newIndex);
				changesMade = true;
			}

			return changesMade;
		}

		private static void AddInputPort(StateNodeModel node, int index, MethodInfo method)
		{
			var attribute = method.GetCustomAttribute<EnterAttribute>();
			var portModel = attribute?.GetPortData(method, index);
			portModel ??= PortModel.MakeDefaultEntryPort(index);

			// Set the port type based on method parameters
			var parameters = method.GetParameters();
			if (parameters.Length == 1)
			{
				portModel.SetPortType(parameters[0].ParameterType);
			}
			else
			{
				portModel.SetPortType(null);
			}

			node.InputPorts.Add(portModel);
		}

		private static void AddOutputPort(StateNodeModel node, int index, EventInfo eventInfo)
		{
			var attribute = eventInfo.GetCustomAttribute<TransitionAttribute>();
			var portModel = attribute.GetPortData(eventInfo, index);

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
	}
}
