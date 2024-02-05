using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Purple), NodeIcon(NodeIcon.V2_Share)]
	public class SubStateMachine : State
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] private StateMachineModel _model;
		
		private StateMachine _subStateMachine;
		private bool _activated;

		public override void Awake()
		{
			CreateStateMachine();
			_activated = _subStateMachine != null;
		}
		
		public override void Start()
		{
			if(!_activated) return;
			_subStateMachine.Start();
		}

		public override void Enter()
		{
			if (!_activated) return;
			
			_subStateMachine.Model.SetParent(StateMachine.Model);
			_subStateMachine.OnComplete += HandleComplete;
			_subStateMachine.Enter();

			#if UNITY_EDITOR
			{
				Selection.activeObject = _subStateMachine.Model;
			}
			#endif 
		}

		public override void Update()
		{
			if (!_activated) return;
			_subStateMachine.Update();
		}

		public override void FixedUpdate()
		{
			if (!_activated) return;
			_subStateMachine.FixedUpdate();
		}

		public override void Exit()
		{
		}

		public override void OnDestroy()
		{
			if (!_activated) return;
			_subStateMachine.OnDestroy();
		}

		private void CreateStateMachine()
		{
			if(_model == null) return;
			_subStateMachine = new StateMachine(_model, this.GameObject);
		}

		private void HandleComplete(State state)
		{
			#if UNITY_EDITOR
			{
				Selection.activeObject = StateMachine.Model;
			}
			#endif 
			
			OnComplete?.Invoke();
		}
	}
}