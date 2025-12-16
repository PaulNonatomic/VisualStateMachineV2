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
		public StateMachineModel Model => _model;

		[SerializeField] private StateMachineModel _model;

		private bool _started;
		private bool _entered;

		public override void OnAwake()
		{
			CreateStateMachine();
			ReplaceModelWithActiveModel();
		}
		
		public override void OnStart()
		{
			SubStateMachine?.Start();
			_started = true;
		}

		[Enter]
		public override void OnEnter()
		{
			if(SubStateMachine == null) return;

			_entered = true;
			SubStateMachine.Model.SetParent(SubStateMachine.Model);
			SubStateMachine.OnComplete += OnSubStateComplete;
			SubStateMachine.Enter();
		}

		public override void OnUpdate()
		{
			SubStateMachine?.Update();
		}

		public override void OnFixedUpdate()
		{
			SubStateMachine?.FixedUpdate();
		}

		public override void OnLateUpdate()
		{
			SubStateMachine?.LateUpdate();
		}

		public override void OnExit()
		{
			if(SubStateMachine == null) return;
			
			SubStateMachine.OnComplete -= OnSubStateComplete;
			SubStateMachine.Exit();
			_entered = false;
		}

		public override void OnDestroy()
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
			ReplaceModelWithActiveModel();

			if (!GameObject.activeInHierarchy || !_started) return;
			SubStateMachine?.Start();

			if (!_entered) return;
			SubStateMachine?.Enter();
		}

		protected virtual void CreateStateMachine()
		{
			if(!_model) return;
			
			SubStateMachine = new StateMachine(_model, Controller, SharedData);
			SubStateMachine.SetParent(StateMachine);
		}

		protected virtual void OnSubStateComplete(State state, StateMachineModel model)
		{
			//...
		}
		
		protected virtual void ReplaceModelWithActiveModel()
		{
			if (SubStateMachine == null) return;
			
			_model = SubStateMachine.Model;
		}

		protected virtual void ReplaceModelWithOriginalModel()
		{
			if (!_model) return;
			
			_model = _model.Original;
		}
	}
}