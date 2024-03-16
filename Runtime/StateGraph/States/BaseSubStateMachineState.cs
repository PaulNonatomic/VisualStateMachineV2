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
		
		public override void OnAwakeState()
		{
			CreateStateMachine();
		}
		
		public override void OnStartState()
		{
			_subSubStateMachine.Start();
		}

		public override void OnEnterState()
		{
			_subSubStateMachine.Model.SetParent(_subSubStateMachine.Model);
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
			_subSubStateMachine.Update();
			
			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel != StateMachine.Model) return;
				ModelSelection.ActiveModel = _subSubStateMachine.Model;
			}
			#endif 
		}

		public override void OnFixedUpdateState()
		{
			_subSubStateMachine.FixedUpdate();
		}

		public override void OnExitState()
		{
			_subSubStateMachine.OnComplete -= HandleComplete;
		}

		public override void OnDestroyState()
		{
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