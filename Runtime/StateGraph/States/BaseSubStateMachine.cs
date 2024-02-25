using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Purple), NodeIcon(NodeIcon.V2_Share), NodeWidth(200)]
	public abstract class BaseSubStateMachineState : State
	{
		public StateMachine StateMachine => _subStateMachine;
		
		[SerializeField] private StateMachineModel _model;
		
		private StateMachine _subStateMachine;
		private bool _activated;

		public override void OnAwakeState()
		{
			CreateStateMachine();
			_activated = _subStateMachine != null;
		}
		
		public override void OnStartState()
		{
			if(!_activated) return;
			_subStateMachine.Start();
		}

		public override void OnEnterState()
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

		public override void OnUpdateState()
		{
			if (!_activated) return;
			_subStateMachine.Update();
		}
		
		public override void OnExitState()
		{
			_subStateMachine.OnComplete -= HandleComplete;
		}

		public override void OnFixedUpdateState()
		{
			if (!_activated) return;
			_subStateMachine.FixedUpdate();
		}
		
		public override void OnDestroyState()
		{
			if (!_activated) return;
			_subStateMachine.OnDestroy();
		}

		protected virtual void CreateStateMachine()
		{
			if(_model == null) return;
			_subStateMachine = new StateMachine(_model, this.GameObject);
		}

		protected virtual void HandleComplete(State state)
		{
			#if UNITY_EDITOR
			{
				Selection.activeObject = StateMachine.Model;
			}
			#endif 
		}
	}
}