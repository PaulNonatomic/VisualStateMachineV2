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
			set => _model = value;
		}

		[SerializeField] private StateMachineModel _model;
		private bool _started;
		private bool _entered;

		public override void OnAwakeState()
		{
			CreateStateMachine();
			ReplaceModelWithActiveModel();
		}
		
		public override void OnStartState()
		{
			SubStateMachine?.Start();
			_started = true;
		}

		[Enter]
		public override void OnEnterState()
		{
			if(SubStateMachine == null) return;

			_entered = true;
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
			_entered = false;
		}

		public override void OnDestroyState()
		{
			ReplaceModelWithOriginalModel();
			
			SubStateMachine?.OnDestroy();
		}
		
		protected virtual void SwitchModel(StateMachineModel value)
		{
			if (!value) return;
			
			SubStateMachine?.OnDestroy();
			SubStateMachine = null;
			
			_model = value;
			CreateStateMachine();

			if (!GameObject.activeInHierarchy || !_started) return;
			SubStateMachine?.Start();

			if (!_entered) return;
			SubStateMachine?.Enter();
		}

		protected virtual void CreateStateMachine()
		{
			if(!_model) return;
			
			SubStateMachine = new StateMachine(_model, GameObject, SharedData);
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