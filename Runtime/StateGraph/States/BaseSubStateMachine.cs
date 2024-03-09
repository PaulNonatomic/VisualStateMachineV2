using Nonatomic.VSM2.NodeGraph;
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
		public StateMachine SubStateMachine => _subSubStateMachine;
		public StateMachineModel Model => _model;
		
		[SerializeField] private StateMachineModel _model;
		
		private StateMachine _subSubStateMachine;
		private bool _activated;

		public override void OnAwakeState()
		{
			CreateStateMachine();
			_activated = _subSubStateMachine != null;
		}
		
		public override void OnStartState()
		{
			if(!_activated) return;
			_subSubStateMachine.Start();
		}

		public override void OnEnterState()
		{
			if (!_activated) return;
			
			_subSubStateMachine.Model.SetParent(SubStateMachine.Model);
			_subSubStateMachine.OnComplete += HandleComplete;
			_subSubStateMachine.Enter();

			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel != StateMachine.Model) return;
				ModelSelection.ActiveModel = _subSubStateMachine.Model;
			}
			#endif 
		}

		public override void OnUpdateState()
		{
			if (!_activated) return;
			_subSubStateMachine.Update();
			
			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel != StateMachine.Model) return;
				ModelSelection.ActiveModel = _subSubStateMachine.Model;
			}
			#endif 
		}
		
		public override void OnExitState()
		{
			_subSubStateMachine.OnComplete -= HandleComplete;
		}

		public override void OnFixedUpdateState()
		{
			if (!_activated) return;
			_subSubStateMachine.FixedUpdate();
		}
		
		public override void OnDestroyState()
		{
			if (!_activated) return;
			_subSubStateMachine.OnDestroy();
		}

		protected virtual void CreateStateMachine()
		{
			if(_model == null) return;
			
			_subSubStateMachine = new StateMachine(_model, this.GameObject);
			_subSubStateMachine.SetParent(StateMachine);
		}

		protected virtual void HandleComplete(State state)
		{
			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel == _subSubStateMachine.Model)
				{
					ModelSelection.ActiveModel = SubStateMachine.Model.Parent;
				}
			}
			#endif
		}
	}
}