using System;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	public class StateMachineController : MonoBehaviour
	{
		[SerializeField] private StateMachineModel _model;
		[SerializeField] private string _id;

		private StateMachine _stateMachine;
		private bool _activated;

		public string Id => _id;
		public StateMachineModel Model => _stateMachine != null ? _stateMachine.Model : _model;

		public virtual void Reset()
		{
			CreateUniqueId();
		}

		public void Awake()
		{
			CreateStateMachine();
			_activated = _stateMachine != null;
		}

		public void Start()
		{
			if(!_activated) return;
			_stateMachine.Start();
			_stateMachine.Enter();
		}

		public void Update()
		{
			if (!_activated) return;
			_stateMachine.Update();
		}

		public void FixedUpdate()
		{
			if (!_activated) return;
			_stateMachine.FixedUpdate();
		}

		public void OnDestroy()
		{
			if (!_activated) return;
			_stateMachine.OnDestroy();
		}

		private void CreateUniqueId()
		{
			if (!string.IsNullOrEmpty(_id)) return;
			_id = Guid.NewGuid().ToString();
		}

		private void CreateStateMachine()
		{
			if(_model == null) return;
			_stateMachine = new StateMachine(_model, gameObject);
		}
	}
}