using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Extensions;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.Utils;

namespace Nonatomic.VSM2.StateGraph.Validation
{
	public static class StateNodeValidator
	{
		public static bool ValidateInputPorts(StateNodeModel node)
		{
			var changesMade = false;
			var type = node.State.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
				.ToList();
			
			var defaultEntryMethod = type.GetMethod("OnEnterState", BindingFlags.Public | BindingFlags.Instance);
			var defaultEntryPort = node.InputPorts.FirstOrDefault(p => string.IsNullOrEmpty(p.PortTypeName));
			
			if (defaultEntryMethod == null)
			{
				if (defaultEntryPort != null)
				{
					GraphLog.LogWarning($"Default input port found, but method is missing so removing port {defaultEntryPort.Id}");
					node.InputPorts.Remove(defaultEntryPort);
					changesMade = true;
				}
			}
			else
			{
				if (defaultEntryPort == null)
				{
					GraphLog.LogWarning($"Default entry method found, but input port is missing so adding port {defaultEntryMethod.Name}");
					var index = ReflectionUtils.GetMethodIndexInType(defaultEntryMethod);
					AddDefaultOnEnterStatePort(node, index);
					changesMade = true;
				}
			}
			
			var methodCount = methods.Count + (defaultEntryMethod != null ? 1 : 0);
			if (methodCount != node.InputPorts.Count)
			{
				GraphLog.LogWarning($"Input port count mismatch, expected {methodCount}, found {node.InputPorts.Count}");
				changesMade |= SynchronizeInputPortData(node, methods, defaultEntryMethod);
			}
			else
			{
				changesMade |= SynchronizeInputPortData(node, methods, defaultEntryMethod);
			}

			return changesMade;
		}

		private static bool SynchronizeInputPortData(StateNodeModel node, List<MethodInfo> methods, MethodInfo defaultEntryMethod)
		{
			var changesMade = false;
			var portDatas = node.InputPorts;
			var methodInfoNames = new HashSet<string>();
			var methodInfoIndices = new Dictionary<string, int>();
			var methodInfoDict = new Dictionary<string, MethodInfo>();
			var indexOffset = defaultEntryMethod != null ? 1 : 0;

			for (var i = 0; i < methods.Count; i++)
			{
				var method = methods[i];
				var methodIndex = i + indexOffset;
				methodInfoNames.Add(method.Name);
				methodInfoIndices[method.Name] = methodIndex;
				methodInfoDict[method.Name] = method;
			}

			var usedIndices = new HashSet<int>();

			for (var i = portDatas.Count - 1; i >= 0; i--)
			{
				var portData = portDatas[i];

				// Handle default entry port separately
				if (string.IsNullOrEmpty(portData.PortTypeName) || portData.Id == "Enter")
				{
					if (defaultEntryMethod == null)
					{
						GraphLog.LogWarning($"Removing default PortData with ID '{portData.Id}' as default method does not exist.");
						portDatas.RemoveAt(i);
						changesMade = true;
					}
					else
					{
						if (portData.Index != 0)
						{
							GraphLog.LogWarning($"Correcting index of default PortData with ID '{portData.Id}' from {portData.Index} to 0.");
							portData.Index = 0;
							changesMade = true;
						}
						usedIndices.Add(0);
					}
					continue;
				}

				if (!methodInfoNames.Contains(portData.Id))
				{
					GraphLog.LogWarning($"Removing PortData with ID '{portData.Id}' as it does not match any method.");
					portDatas.RemoveAt(i);
					changesMade = true;
				}
				else
				{
					var correctIndex = methodInfoIndices[portData.Id];
					if (portData.Index != correctIndex)
					{
						GraphLog.LogWarning($"Correcting index of PortData with ID '{portData.Id}' from {portData.Index} to {correctIndex}.");
						portData.Index = correctIndex;
						changesMade = true;
					}

					if (usedIndices.Contains(portData.Index))
					{
						GraphLog.LogWarning($"Duplicate index found for PortData with ID '{portData.Id}'. Removing entry.");
						portDatas.RemoveAt(i);
						changesMade = true;
					}
					else
					{
						usedIndices.Add(portData.Index);
					}

					var methodInfo = methodInfoDict[portData.Id];
					var parameters = methodInfo.GetParameters();

					string expectedPortTypeName;
					Type expectedPortType;
					
					if (parameters.Length == 0)
					{
						expectedPortTypeName = string.Empty;
						expectedPortType = null;
					}
					else if (parameters.Length == 1)
					{
						var parameterType = parameters[0].ParameterType;
						expectedPortTypeName = parameterType.GetSimplifiedName() ?? string.Empty;
						expectedPortType = parameterType;
					}
					else
					{
						GraphLog.LogWarning($"Method '{methodInfo.Name}' has more than one parameter, which is not supported.");
						expectedPortTypeName = string.Empty;
						expectedPortType = null;
					}

					if (portData.PortTypeName != expectedPortTypeName)
					{
						GraphLog.LogWarning($"PortTypeName mismatch for Port with ID '{portData.Id}'. Expected '{expectedPortTypeName}', found '{portData.PortTypeName}'");
						portData.SetPortType(expectedPortType);
						changesMade = true;
					}
				}
			}

			// Add missing PortData items
			foreach (var method in methods)
			{
				if (portDatas.Any(pd => pd.Id == method.Name)) continue;

				var newIndex = methodInfoIndices[method.Name];
				if (usedIndices.Contains(newIndex)) continue;

				var attribute = method.GetCustomAttribute<EnterAttribute>();
				if (attribute == null) continue;

				GraphLog.LogWarning($"Adding missing PortData for Method '{method.Name}' at index {newIndex}.");
				var portModel = attribute.GetPortData(method, newIndex);

				// Get the method parameter type
				var parameters = method.GetParameters();

				if (parameters.Length == 1)
				{
					portModel.SetPortType(parameters[0].ParameterType);
				}
				else
				{
					portModel.SetPortType(null);
				}

				portDatas.Add(portModel);
				usedIndices.Add(newIndex);
				changesMade = true;
			}

			// Handle default entry method if not already handled
			if (defaultEntryMethod != null && !usedIndices.Contains(0))
			{
				// Default entry port is missing, add it
				AddDefaultOnEnterStatePort(node, 0);
				GraphLog.LogWarning($"Adding missing default entry PortData at index 0.");
				changesMade = true;
			}

			return changesMade;
		}

		public static bool ValidateOutputPorts(StateNodeModel node)
		{
			var changesMade = false;
			var type = node.State.GetType();
			var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			changesMade |= SynchronizeOutputPortData(node, events);
			return changesMade;
		}

		private static bool SynchronizeOutputPortData(StateNodeModel node, EventInfo[] eventInfos)
		{
			bool changesMade = false;
			var portDatas = node.OutputPorts;
			var eventInfoNames = new HashSet<string>();
			var eventInfoIndices = new Dictionary<string, int>();
			var eventInfoDict = new Dictionary<string, EventInfo>();

			// Build dictionaries for quick lookup
			for (var i = 0; i < eventInfos.Length; i++)
			{
				var eventInfo = eventInfos[i];
				eventInfoNames.Add(eventInfo.Name);
				eventInfoIndices[eventInfo.Name] = i;
				eventInfoDict[eventInfo.Name] = eventInfo;
			}

			// Track indices already used to ensure uniqueness
			var usedIndices = new HashSet<int>();

			// Synchronize existing PortData items
			for (var i = portDatas.Count - 1; i >= 0; i--)
			{
				var portData = portDatas[i];
				if (!eventInfoNames.Contains(portData.Id))
				{
					// PortData ID does not match any EventInfo name
					GraphLog.LogWarning($"Removing PortData with ID '{portData.Id}' as it does not match any EventInfo.");
					portDatas.RemoveAt(i);
					changesMade = true;
				}
				else
				{
					// Check if the index is correct
					var correctIndex = eventInfoIndices[portData.Id];
					if (portData.Index != correctIndex)
					{
						GraphLog.LogWarning($"Correcting index of PortData with ID '{portData.Id}' from {portData.Index} to {correctIndex}.");
						portData.Index = correctIndex;
						changesMade = true;
					}

					// Ensure uniqueness of indices
					if (usedIndices.Contains(portData.Index))
					{
						GraphLog.LogWarning($"Duplicate index found for PortData with ID '{portData.Id}'. Removing entry.");
						portDatas.RemoveAt(i);
						changesMade = true;
					}
					else
					{
						usedIndices.Add(portData.Index);
					}

					// Get the EventInfo corresponding to this PortData
					var eventInfo = eventInfoDict[portData.Id];
					var handlerType = eventInfo.EventHandlerType;
					var invokeMethod = handlerType.GetMethod("Invoke");
					var parameters = invokeMethod.GetParameters();

					// Get the expected PortTypeName from the event's parameter type
					string expectedPortTypeName;
					Type expectedPortType;
					if (parameters.Length == 0)
					{
						expectedPortTypeName = string.Empty;
						expectedPortType = null;
					}
					else if (parameters.Length == 1)
					{
						var parameterType = parameters[0].ParameterType;
						expectedPortTypeName = parameterType.GetSimplifiedName() ?? string.Empty;
						expectedPortType = parameterType;
					}
					else
					{
						// More than one parameter, not expected
						GraphLog.LogWarning($"Event '{eventInfo.Name}' has more than one parameter, which is not supported.");
						expectedPortTypeName = string.Empty;
						expectedPortType = null;
					}

					// Now compare portData.PortTypeName to expectedPortTypeName
					if (portData.PortTypeName != expectedPortTypeName)
					{
						GraphLog.LogWarning($"PortTypeName mismatch for PortData with ID '{portData.Id}'. Expected '{expectedPortTypeName}', found '{portData.PortTypeName}'.");
						portData.SetPortType(expectedPortType);
						changesMade = true;
					}
				}
			}

			// Add missing PortData items
			foreach (var eventInfo in eventInfos)
			{
				if (portDatas.Any(pd => pd.Id == eventInfo.Name)) continue;

				var newIndex = eventInfoIndices[eventInfo.Name];
				if (usedIndices.Contains(newIndex)) continue;

				var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
				if (attributes.Length == 0) continue;

				GraphLog.LogWarning($"Adding missing PortData for EventInfo '{eventInfo.Name}' at index {newIndex}.");
				var attribute = (TransitionAttribute)attributes[0];
				var portModel = attribute.GetPortData(eventInfo, newIndex);

				// Get the event parameter type
				var handlerType = eventInfo.EventHandlerType;
				var invokeMethod = handlerType.GetMethod("Invoke");
				var parameters = invokeMethod.GetParameters();

				if (parameters.Length == 1)
				{
					portModel.SetPortType(parameters[0].ParameterType);
				}
				else
				{
					portModel.SetPortType(null);
				}

				portDatas.Add(portModel);
				usedIndices.Add(newIndex);
				changesMade = true;
			}

			return changesMade;
		}

		private static void AddDefaultOnEnterStatePort(StateNodeModel node, int index)
		{
			if (index < 0) return;

			node.InputPorts.Add(new PortModel()
			{
				Id = "Enter",
				PortLabel = "Enter",
				Index = index
			});
		}
	}
}
