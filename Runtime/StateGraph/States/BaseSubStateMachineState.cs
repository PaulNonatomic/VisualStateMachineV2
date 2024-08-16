using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Purple), NodeIcon(NodeIcon.Share), NodeWidth(200)]
	public abstract class BaseSubStateMachineState : State
	{
		public StateMachine SubStateMachine { get; private set; }

		public StateMachineModel Model
		{
			get => _model;
			protected set => _model = value;
		}

		[SerializeField] private StateMachineModel _model;

		public override void OnAwakeState()
		{
			CreateStateMachine();
			ReplaceModelWithActiveModel();
		}
		
		public override void OnStartState()
		{
			SubStateMachine?.Start();
		}

		public override void OnEnterState()
		{
			if(SubStateMachine == null) return;
			
			SubStateMachine.Model.SetParent(SubStateMachine.Model);
			SubStateMachine.OnComplete += OnSubStateComplete;
			SubStateMachine.Enter();
		}

		public override void OnUpdateState()
		{
			SubStateMachine?.Update();
		}

		public override void OnFixedUpdateState()
		{
			SubStateMachine?.FixedUpdate();
		}

		public override void OnExitState()
		{
			if(SubStateMachine == null) return;
			
			SubStateMachine.OnComplete -= OnSubStateComplete;
			SubStateMachine.Exit();
		}

		public override void OnDestroyState()
		{
			ReplaceModelWithOriginalModel();
			
			SubStateMachine?.OnDestroy();
		}

		protected virtual void CreateStateMachine()
		{
			if(!_model) return;
			
			SubStateMachine = new StateMachine(_model, this.GameObject);
			SubStateMachine.SetParent(StateMachine);
		}

		protected virtual void OnSubStateComplete(State state, StateMachineModel model)
		{
			//...
		}
		
		private void ReplaceModelWithActiveModel()
		{
			if (SubStateMachine == null) return;
			
			_model = SubStateMachine.Model;
		}

		private void ReplaceModelWithOriginalModel()
		{
			if (!_model) return;
			
			_model = _model.Original;
		}
	}
}