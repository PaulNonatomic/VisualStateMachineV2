using System;
using Nonatomic.VSM2.Data;
using Nonatomic.VSM2.StateGraph.States;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	/// <summary>
	/// Controls a state machine within a Unity GameObject, managing its lifecycle and model.
	/// </summary>
	public class StateMachineController : MonoBehaviour
	{
		/// <summary>
		/// Gets the unique identifier for this controller.
		/// </summary>
		public string Id => _id;

		public State State => _stateMachine?.State;
		public ISharedData SharedData => _stateMachine.SharedData;
		
		/// <summary>
		/// Gets the current StateMachineModel, either from the active state machine or the serialized field.
		/// </summary>
		public StateMachineModel Model => _stateMachine != null 
			? _stateMachine.Model 
			: _model;

		[SerializeField] private StateMachineModel _model;
		[SerializeField, HideInInspector] private string _id;
		
		private StateMachine _stateMachine;
		private bool _activated;
		private bool _started;

		/// <summary>
		/// Jumps to a state via a JumpIn state using the JumpId to identify the JumpIn node.
		/// </summary>
		/// <param name="id">The JumpIn id of the destination JumpIn node</param>
		public virtual void JumpTo(JumpId id)
		{
			Debug.Log("JumpTo: " + id);
			_stateMachine?.JumpTo(id);
		}

		/// <summary>
		/// Changes the StateMachineModel and reinitializes the state machine.
		/// If the GameObject is active and has already started, it will also call Start and Enter on the new state machine.
		/// </summary>
		/// <param name="value">The new StateMachineModel to set.</param>
		public virtual void SwitchModel(StateMachineModel value)
		{
			Debug.Log("SwitchModel: " + value.ModelName);
			if (!value) return;
			
			_stateMachine?.OnDestroy();
			_stateMachine = null;
			
			_model = value;
			CreateStateMachine();

			if (!gameObject.activeInHierarchy || !_started) return;
			_stateMachine?.Start();
			_stateMachine?.Enter();
		}
		
		/// <summary>
		/// Resets the controller by creating a new unique identifier.
		/// This method is called when the script is added to a GameObject or reset in the inspector.
		/// </summary>
		public virtual void Reset()
		{
			CreateUniqueId();
		}

		/// <summary>
		/// Initializes the state machine when the script instance is being loaded.
		/// </summary>
		public virtual void Awake()
		{
			CreateStateMachine();
			_activated = _stateMachine != null;
		}

		/// <summary>
		/// Starts the state machine if it has been activated.
		/// This is called on the frame when a script is enabled just before any of the Update methods are called the first time.
		/// </summary>
		public virtual void Start()
		{
			if(!_activated) return;
			
			Debug.Log("StateMachineController.Start");
			_stateMachine?.Start();
			_stateMachine?.Enter();
			_started = true;
		}

		/// <summary>
		/// Updates the state machine if it has been activated.
		/// This is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		public virtual void Update()
		{
			if (!_activated) return;
			_stateMachine?.Update();
		}

		/// <summary>
		/// Performs physics-based updates on the state machine if it has been activated.
		/// This is called every fixed framerate frame, if the MonoBehaviour is enabled.
		/// </summary>
		public virtual void FixedUpdate()
		{
			if (!_activated) return;
			_stateMachine?.FixedUpdate();
		}

		/// <summary>
		/// Invokes the LateUpdate method of the current state machine if it is activated.
		/// This method is called once per frame after all Update calls have been completed.
		/// </summary>
		public virtual void LateUpdate()
		{
			if (!_activated) return;
			_stateMachine?.LateUpdate();
		}

		/// <summary>
		/// Cleans up the state machine when the MonoBehaviour will be destroyed.
		/// </summary>
		public virtual void OnDestroy()
		{
			if (!_activated) return;
			_stateMachine?.OnDestroy();
		}

		/// <summary>
		/// Creates a unique identifier for this controller if one doesn't already exist.
		/// </summary>
		private void CreateUniqueId()
		{
			if (!string.IsNullOrEmpty(_id)) return;
			_id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Creates a new StateMachine instance using the current model and GameObject.
		/// </summary>
		private void CreateStateMachine()
		{
			if(!_model) return;
			_stateMachine = new StateMachine(_model, this);
		}
		
		private void OnValidate()
		{
			CreateUniqueId();
		}
	}
}