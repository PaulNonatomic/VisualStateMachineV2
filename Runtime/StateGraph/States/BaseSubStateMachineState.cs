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
		public StateMachine SubStateMachine { get; private set; }
		public StateMachineModel Model => _model;
		
		[SerializeField] private StateMachineModel _model;

		public override void OnAwakeState()
		{
			CreateStateMachine();
		}
		
		public override void OnStartState()
		{
			SubStateMachine.Start();
		}

		public override void OnEnterState()
		{
			SubStateMachine.Model.SetParent(SubStateMachine.Model);
			SubStateMachine.OnComplete += OnSubStateComplete;
			SubStateMachine.Enter();

			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel != StateMachine.Model) return;
				ModelSelection.ActiveModel = SubStateMachine.Model;
			}
			#endif 
		}

		public override void OnUpdateState()
		{
			SubStateMachine.Update();
			
			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel != StateMachine.Model) return;
				ModelSelection.ActiveModel = SubStateMachine.Model;
			}
			#endif 
		}

		public override void OnFixedUpdateState()
		{
			SubStateMachine.FixedUpdate();
		}

		public override void OnExitState()
		{
			SubStateMachine.OnComplete -= OnSubStateComplete;
			SubStateMachine.Exit();
		}

		public override void OnDestroyState()
		{
			SubStateMachine.OnDestroy();
		}

		protected virtual void CreateStateMachine()
		{
			if(_model == null) return;
			
			SubStateMachine = new StateMachine(_model, this.GameObject);
			SubStateMachine.SetParent(StateMachine);
		}

		protected virtual void OnSubStateComplete(State state)
		{
			#if UNITY_EDITOR
			{
				if (ModelSelection.ActiveModel == SubStateMachine.Model)
				{
					ModelSelection.ActiveModel = SubStateMachine.Model.Parent;
				}
			}
			#endif
		}
	}
}