using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Purple), NodeIcon(NodeIcon.V2_Share, opacity:0.8f)]
	public class SubStateMachine : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] private StateMachineModel _model;
		
		private StateMachine _stateMachine;
		private bool _activated;

		public override void Awake()
		{
			CreateStateMachine();
			_activated = _stateMachine != null;
		}
		
		public override void Start()
		{
			if(!_activated) return;
			_stateMachine.Start();
		}

		public override void Enter()
		{
			if (!_activated) return;
			_stateMachine.OnComplete += HandleComplete;
			_stateMachine.Enter();
		}

		public override void Update()
		{
			if (!_activated) return;
			_stateMachine.Update();
		}

		public override void FixedUpdate()
		{
			if (!_activated) return;
			_stateMachine.FixedUpdate();
		}

		public override void Exit()
		{
		}

		public override void OnDestroy()
		{
			if (!_activated) return;
			_stateMachine.OnDestroy();
		}

		private void CreateStateMachine()
		{
			if(_model == null) return;
			_stateMachine = new StateMachine(_model, this.GameObject);
		}

		private void HandleComplete(State state)
		{
			OnComplete?.Invoke();
		}
	}
}