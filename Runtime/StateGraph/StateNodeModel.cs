using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nonatomic.VSM2.Logging;
using Nonatomic.VSM2.NodeGraph;
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

		public void Enter()
		{
			if (Active) return;
			
			Active = true;
			LastActive = Time.time;
			State?.OnEnterState();
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

		public void ValidatePorts()
		{
			var type = State.GetType();
			var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			
			SynchronizePortData(events, OutputPorts);
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
		
		private void ToggleEventSubscriptionByName(object targetObject, string eventName, StateTransitionModel transition, bool subscribe)
		{
			var eventInfo = targetObject.GetType().GetEvent(eventName);
			if (eventInfo == null) return;
			
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, transition, nameof(transition.Transition));

			if (subscribe)
			{
				eventInfo.AddEventHandler(targetObject, handler);
			}
			else
			{
				eventInfo.RemoveEventHandler(targetObject, handler);
			}
		}

		private void SynchronizePortData(EventInfo[] eventInfos, List<PortModel> portDatas)
		{
			var eventInfoNames = new HashSet<string>();
			var eventInfoIndices = new Dictionary<string, int>();

			// Build dictionaries for quick lookup
			for (var i = 0; i < eventInfos.Length; i++)
			{
				eventInfoNames.Add(eventInfos[i].Name);
				eventInfoIndices[eventInfos[i].Name] = i;
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
				}
				else
				{
					// Check if the index is correct
					var correctIndex = eventInfoIndices[portData.Id];
					if (portData.Index != correctIndex)
					{
						GraphLog.LogWarning($"Correcting index of PortData with ID '{portData.Id}' from {portData.Index} to {correctIndex}.");
						portData.Index = correctIndex;
					}

					// Ensure uniqueness of indices
					if (usedIndices.Contains(portData.Index))
					{
						GraphLog.LogWarning($"Duplicate index found for PortData with ID '{portData.Id}'. Removing entry.");
						portDatas.RemoveAt(i);
					}
					else
					{
						usedIndices.Add(portData.Index);
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
				var attribute = (TransitionAttribute) attributes[0];
				var portModel = attribute.GetPortData(eventInfo, newIndex);
				
				portDatas.Add(portModel);
				usedIndices.Add(newIndex);
			}
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
			InputPorts.Add(new PortModel()
			{
				Id = "OnEnterState",
				Index = 0
			});
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
	}
}