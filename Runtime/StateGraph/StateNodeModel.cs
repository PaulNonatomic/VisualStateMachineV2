using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Extensions;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nonatomic.VSM2.StateGraph
{
	[Serializable]
	public class StateNodeModel : NodeModel
	{
		public State State;
		public bool Active { get; private set; }
		public bool Enabled { get; private set; }
		public float LastActive { get; private set; }
	
		public StateNodeModel(State state, Vector2 position)
		{
			Id = state.name;
			State = state;
			Position = position;
			
			CreateInputPorts(state);
			CreateOutputPorts(state);
		}

		public void Awake()
		{
			if (Enabled) return;
			
			Enabled = true;
			LastActive = -1;
			State?.OnAwakeState();
		}

		public void Start()
		{
			State?.OnStartState();
		}

		public void Enter(TransitionEventData eventData)
		{
			if (Active) return;
			
			Active = true;
			LastActive = Time.time;
			State.TransitionData = eventData;
			
			if (eventData.HasValue)
			{
				var type = State.GetType();
				var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
					.ToList();

				var method = methods.FirstOrDefault(m => 
					m.GetParameters().Length == 1 && 
					m.GetParameters()[0].ParameterType == eventData.Type);
				
				if (method != null)
				{
					try
					{
						method.Invoke(this, new[] { eventData.Value });
					}
					catch (Exception ex)
					{
						Debug.LogError($"Error invoking OnEnterState method: {ex.Message}");
					}
				}
				else
				{
					// If no matching method is found, call the parameterless OnEnterState
					State?.OnEnterState();
				}
			}
			else
			{
				State?.OnEnterState();
			}
		}

		public void Update()
		{
			LastActive = Time.time;
			State?.OnUpdateState();
		}

		public void FixedUpdate()
		{
			State?.OnFixedUpdateState();
		}

		public void Exit()
		{
			if (!Active) return;
			
			LastActive = Time.time;
			Active = false;
			State?.OnExitState();
		}

		public void OnDestroy()
		{
			if (!Enabled) return;
			
			Active = false;
			Enabled = false;
			State?.OnDestroyState();
		}

		public bool ValidateInputPorts()
		{
			bool changesMade = false;
			GraphLog.Log($"//////////////////////////////");
			GraphLog.Log($"Validate Input Ports on Node {this.Id}");
			
			var type = State.GetType();
		
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
				.ToList();
			
			var defaultEntryMethod = type.GetMethod("OnEnterState", BindingFlags.Public | BindingFlags.Instance);
			var defaultEntryPort = InputPorts.FirstOrDefault(p => string.IsNullOrEmpty(p.PortTypeName));
			
			if (defaultEntryMethod == null)
			{
				//No default entry method found so lets double check there isn't already one in the inputports list
				if (defaultEntryPort != null)
				{
					GraphLog.LogWarning($"Default input port found, but method is missing so removing port {defaultEntryPort.Id}");
					InputPorts.Remove(defaultEntryPort);
					changesMade = true;
				}
			}
			else
			{
				//Default entry method found, lets check if we have a port for it
				if (defaultEntryPort == null)
				{
					GraphLog.LogWarning($"Default entry method found, but input port is missing so adding port {defaultEntryMethod.Name}");
					var index = ReflectionUtils.GetMethodIndexInType(defaultEntryMethod);
					AddDefaultOnEnterStatePort(index);
					changesMade = true;
				}
			}
			
			//Do we have the same number of ports as we do methods
			var methodCount = methods.Count + (defaultEntryMethod != null ? 1 : 0);
			if(methodCount != InputPorts.Count)
			{
				GraphLog.LogWarning($"Input port count mismatch, expected {methods.Count}, found {InputPorts.Count}");
				changesMade |= SynchronizeInputPortData(methods, defaultEntryMethod, InputPorts);
			}
			else
			{
				changesMade |= SynchronizeInputPortData(methods, defaultEntryMethod, InputPorts);
			}
			
			return changesMade;
		}

		private bool SynchronizeInputPortData(List<MethodInfo> methods, MethodInfo defaultEntryMethod, List<PortModel> portDatas)
		{
			bool changesMade = false;
			var methodInfoNames = new HashSet<string>();
			var methodInfoIndices = new Dictionary<string, int>();
			var methodInfoDict = new Dictionary<string, MethodInfo>();

			// Build dictionaries for quick lookup
			int indexOffset = defaultEntryMethod != null ? 1 : 0;

			for (var i = 0; i < methods.Count; i++)
			{
				var method = methods[i];
				var methodIndex = i + indexOffset;
				methodInfoNames.Add(method.Name);
				methodInfoIndices[method.Name] = methodIndex;
				methodInfoDict[method.Name] = method;
			}

			// Track indices already used to ensure uniqueness
			var usedIndices = new HashSet<int>();

			// Synchronize existing PortData items
			for (var i = portDatas.Count - 1; i >= 0; i--)
			{
				var portData = portDatas[i];

				// Handle default entry port separately
				if (string.IsNullOrEmpty(portData.PortTypeName) || portData.Id == "Enter")
				{
					if (defaultEntryMethod == null)
					{
						// Default method doesn't exist, remove the port
						GraphLog.LogWarning($"Removing default PortData with ID '{portData.Id}' as default method does not exist.");
						portDatas.RemoveAt(i);
						changesMade = true;
					}
					else
					{
						// Ensure index is correct
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
					// PortData ID does not match any MethodInfo name
					GraphLog.LogWarning($"Removing PortData with ID '{portData.Id}' as it does not match any method.");
					portDatas.RemoveAt(i);
					changesMade = true;
				}
				else
				{
					// Check if the index is correct
					var correctIndex = methodInfoIndices[portData.Id];
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

					// Get the MethodInfo corresponding to this PortData
					var methodInfo = methodInfoDict[portData.Id];
					var parameters = methodInfo.GetParameters();

					// Get the expected PortTypeName from the method's parameter type
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
						GraphLog.LogWarning($"Method '{methodInfo.Name}' has more than one parameter, which is not supported.");
						expectedPortTypeName = string.Empty;
						expectedPortType = null;
					}

					// Now compare portData.PortTypeName to expectedPortTypeName
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
				AddDefaultOnEnterStatePort(0);
				GraphLog.LogWarning($"Adding missing default entry PortData at index 0.");
				changesMade = true;
			}

			return changesMade;
		}
		
		public bool ValidateOutputPorts()
		{
			bool changesMade = false;
			GraphLog.Log($"//////////////////////////////");
			GraphLog.Log($"Validate Output Ports on Node {this.Id}");
	
			var type = State.GetType();
			var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
	
			changesMade |= SynchronizeOutputPortData(events, OutputPorts);
			return changesMade;
		}

		public StateNodeModel Clone()
		{
			var clone = (StateNodeModel) this.MemberwiseClone();
			
			if (State)
			{
				clone.State = Object.Instantiate(State);
			}

			clone.InputPorts = new List<PortModel>();
			foreach (var inputPort in InputPorts)
			{
				clone.InputPorts.Add(inputPort.Clone());
			}

			clone.OutputPorts = new List<PortModel>();
			foreach (var outputPort in OutputPorts)
			{
				clone.OutputPorts.Add(outputPort.Clone()); // Assuming PortModel has a Clone method
			}

			return clone;
		}

		private bool SynchronizeOutputPortData(EventInfo[] eventInfos, List<PortModel> portDatas)
		{
			bool changesMade = false;
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
		
		private void LookForPortInfoMismatch(EventInfo[] events)
		{
			//Check for port count mismatch
			if (events.Length > OutputPorts.Count)
			{
				GraphLog.LogWarning($"{events.Length - OutputPorts.Count} Missing port from state node: {Id}");
				
				//find missing port
				for (var index = 0; index < events.Length; index++)
				{
					var eventInfo = events[index];
					if (OutputPorts.Any(port => eventInfo.Name == port.Id && index == port.Index)) continue;

					GraphLog.LogWarning($"Missing port: {eventInfo.Name} at index {index}");
				}
			}
			else if(events.Length < OutputPorts.Count)
			{
				GraphLog.LogWarning($"State node contains {OutputPorts.Count - events.Length} deleted ports: {Id}");
				
				//find deleted port
				foreach(var port in OutputPorts)
				{
					if (events.Any(e => e.Name == port.Id)) continue;
					
					GraphLog.LogWarning($"Deleted port: {port.Id}");
				}
			}
			
			for (var index = 0; index < events.Length; index++)
			{
				var eventInfo = events[index];

				if (index >= OutputPorts.Count) break;
				var port = OutputPorts[index];

				if (port.Id != eventInfo.Name)
				{
					GraphLog.LogWarning($"Port Id mismatch: {port.Id}, {eventInfo.Name}");
				}

				if (port.Index != index)
				{
					GraphLog.LogWarning($"Port index mismatch: {port.Index}, {index}");
				}
			}
		}

		private void CreateInputPorts(State state)
		{
			var type = state.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null)
				.ToList();

			if (methods.Count == 0)
			{
				AddDefaultOnEnterStatePort(0);
				return;
			}
			
			for (var i = 0; i < methods.Count; i++)
			{
				var method = methods[i];
				var attribute = method.GetCustomAttribute<EnterAttribute>();
				var portModel = attribute.GetPortData(method, methodIndex:i);
				InputPorts.Add(portModel);
			}
		}

		private void CreateOutputPorts(State state)
		{
			var type = state.GetType();
			var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			
			foreach (var eventInfo in events)
			{
				var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
				if (attributes.Length == 0) continue;

				var eventType = eventInfo.EventHandlerType;
				
				if (!eventType.IsGenericType || eventType.GetGenericTypeDefinition() != typeof(Action<>))
				{
					if (eventType != typeof(Action)) continue;
				}
				
				var attribute = (TransitionAttribute)attributes[0];
				var portModel = attribute.GetPortData(eventInfo, OutputPorts.Count);
				OutputPorts.Add(portModel);
			}
		}

		private void AddDefaultOnEnterStatePort(int index)
		{
			if (index < 0) return;
			
			InputPorts.Add(new PortModel()
			{
				Id = "Enter",
				PortLabel = "Enter",
				Index = index
			});
		}
	}
}