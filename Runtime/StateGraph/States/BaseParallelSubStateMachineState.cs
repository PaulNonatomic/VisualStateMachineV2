using System.Collections.Generic;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	public enum ParallelCompletionMode
	{
		Any,
		All
	}
	
	[NodeColor(NodeColor.Purple), NodeIcon(NodeIcon.V2_Share), NodeWidth(200)]
	public abstract class BaseParallelSubStateMachineState : State
	{
		[SerializeField] protected ParallelCompletionMode CompletionMode = ParallelCompletionMode.Any;
		[SerializeField] protected List<StateMachineModel> Models;
		
		private List<StateMachine> _subSubStateMachines = new();
		
		public override void OnAwakeState()
		{
			CreateStateMachines();
		}
		
		public override void OnStartState()
		{
			foreach (var subSubStateMachine in _subSubStateMachines)
			{
				subSubStateMachine.Start();
			}
		}
		
		public override void OnEnterState()
		{
			foreach(var subSubStateMachine in _subSubStateMachines)
			{
				subSubStateMachine.Model.SetParent(subSubStateMachine.Model);
				subSubStateMachine.OnComplete += HandleComplete;
				subSubStateMachine.Enter();
			}
		}
		
		public override void OnUpdateState()
		{
			foreach(var subSubStateMachine in _subSubStateMachines)
			{
				subSubStateMachine.Update();
			}
		}

		public override void OnFixedUpdateState()
		{
			foreach(var subSubStateMachine in _subSubStateMachines)
			{
				subSubStateMachine.FixedUpdate();
			}
		}

		public override void OnExitState()
		{
			foreach(var subSubStateMachine in _subSubStateMachines)
			{
				subSubStateMachine.OnComplete -= HandleComplete;
				subSubStateMachine.OnDestroy();
			}
		}

		public override void OnDestroyState()
		{
			foreach(var subSubStateMachine in _subSubStateMachines)
			{
				subSubStateMachine.OnDestroy();
			}
		}

		protected virtual void CreateStateMachines()
		{
			_subSubStateMachines.Clear();
			
			foreach (var model in Models)
			{
				if (model == null) continue;
				
				var subSubStateMachine = new StateMachine(model, this.GameObject);
				subSubStateMachine.SetParent(StateMachine);
				_subSubStateMachines.Add(subSubStateMachine);
			}
		}

		protected virtual void HandleComplete(State state)
		{
			//...
		}
	}
}