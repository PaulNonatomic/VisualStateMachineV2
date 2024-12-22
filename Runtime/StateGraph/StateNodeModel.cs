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
			State?.OnAwake();
		}

		public void Start()
		{
			State?.OnStart();
		}

		public void Enter(TransitionEventData eventData)
		{
			if (Active) return;
			
			Active = true;
			LastActive = Time.time;
			State.TransitionData = eventData;
			
			var type = State.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null && !m.IsAbstract)
				.ToList();
			
			if (eventData.HasValue)
			{
				var method = methods.FirstOrDefault(m => 
					m.GetParameters().Length == 1 && 
					m.GetParameters()[0].ParameterType == eventData.Type);
				
				if (method != null)
				{
					try
					{
						method.Invoke(State, new[] { eventData.Value });
					}
					catch (Exception ex)
					{
						Debug.LogError($"Error invoking OnEnter method: {ex.Message}");
					}
				}
			}
			else
			{
				var method = methods.FirstOrDefault(m => 
					m.GetParameters().Length == 0);
				
				if (method != null)
				{
					try
					{
						method.Invoke(State, null);
					}
					catch (Exception ex)
					{
						Debug.LogError($"Error invoking OnEnter method: {ex.Message}");
					}
				}
			}
		}

		public void Update()
		{
			LastActive = Time.time;
			State?.OnUpdate();
		}

		public void FixedUpdate()
		{
			State?.OnFixedUpdate();
		}

		public void LateUpdate()
		{
			State?.OnLateUpdate();
		}

		public void Exit()
		{
			if (!Active) return;
			
			LastActive = Time.time;
			Active = false;
			State?.OnExit();
		}

		public void OnDestroy()
		{
			if (!Enabled) return;
			
			Active = false;
			Enabled = false;
			State?.OnDestroy();
		}
		
		public StateNodeModel Clone()
		{
			var clone = (StateNodeModel)this.MemberwiseClone();
			
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
				clone.OutputPorts.Add(outputPort.Clone());
			}

			return clone;
		}

		private void CreateInputPorts(State state)
		{
			var type = state.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null && !m.IsAbstract)
				.ToList();

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
	}
}