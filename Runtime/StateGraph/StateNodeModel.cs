using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			Debug.Log($"{State.name}.OnAwake()");
			State?.OnAwake();
		}

		public void Start()
		{
			Debug.Log($"{State.name}.OnStart()");
			State?.OnStart();
		}

		public void Enter(TransitionEventData eventData)
		{
			Debug.Log($"Entering node {Id}, current Active state: {Active}");
			if (Active) return;
			
			Active = true;
			LastActive = Time.time;
			State.TransitionData = eventData;
			Debug.Log($"Node {Id} Active set to: {Active}");
			
			var type = State.GetType();
			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
				.Where(m => m.GetCustomAttribute<EnterAttribute>() != null && !m.IsAbstract)
				.ToList();
			Debug.Log($"{State.name} methods");
			
			if (eventData.HasValue)
			{
				var method = methods.FirstOrDefault(m => 
					m.GetParameters().Length == 1 && 
					m.GetParameters()[0].ParameterType == eventData.Type);

				if (method == null) return;
				
				try
				{
					Debug.Log($"{State.name} {method.Name}.Invoke()");
					method.Invoke(State, new[] { eventData.Value });
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			else
			{
				var method = methods.FirstOrDefault(m => 
					m.GetParameters().Length == 0);

				if (method == null) return;
				
				try
				{
					Debug.Log($"{State.name} {method.Name}.Invoke()");
					method.Invoke(State, null);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		public void Update()
		{
			Debug.Log($"{State.name}.OnUpdate()");
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
			Debug.Log($"{State.name}.OnExit()");
			State?.OnExit();
		}

		public void OnDestroy()
		{
			if (!Enabled) return;
			
			Active = false;
			Enabled = false;
			Debug.Log($"{State.name}.OnDestroy()");
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