using System.Collections.Generic;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	[NodeColor(NodeColor.Purple), NodeIcon(NodeIcon.V2_Share), NodeWidth(200)]
	public abstract class BaseParallelSubStateMachineState : State
	{
		public List<StateMachine> SubStateMachines { get; private set; }
		
		[SerializeField] protected List<StateMachineModel> Models;
		
		public override void OnAwakeState()
		{
			CreateStateMachines();
			ReplaceModelsWithActiveModels();
		}

		public override void OnStartState()
		{
			foreach (var subSubStateMachine in SubStateMachines)
			{
				subSubStateMachine.Start();
			}
		}

		public override void OnEnterState()
		{
			foreach(var subSubStateMachine in SubStateMachines)
			{
				subSubStateMachine.Model.SetParent(subSubStateMachine.Model);
				subSubStateMachine.OnComplete += OnSubStateComplete;
				subSubStateMachine.Enter();
			}
		}

		public override void OnUpdateState()
		{
			foreach(var subSubStateMachine in SubStateMachines)
			{
				subSubStateMachine.Update();
			}
		}

		public override void OnFixedUpdateState()
		{
			foreach(var subSubStateMachine in SubStateMachines)
			{
				subSubStateMachine.FixedUpdate();
			}
		}

		public override void OnExitState()
		{
			foreach(var subSubStateMachine in SubStateMachines)
			{
				subSubStateMachine.OnComplete -= OnSubStateComplete;
				subSubStateMachine.Exit();
			}
		}

		public override void OnDestroyState()
		{
			ReplaceModelsWithOriginalModels();
				
			foreach(var subSubStateMachine in SubStateMachines)
			{
				subSubStateMachine.OnDestroy();
			}
		}

		protected virtual void CreateStateMachines()
		{
			SubStateMachines = new List<StateMachine>();
			
			foreach (var model in Models)
			{
				if (model == null) continue;
				
				var subSubStateMachine = new StateMachine(model, this.GameObject);
				subSubStateMachine.SetParent(StateMachine);
				SubStateMachines.Add(subSubStateMachine);
			}
		}

		protected virtual void OnSubStateComplete(State state, StateMachineModel model)
		{
			//...
		}

		private void ReplaceModelsWithActiveModels()
		{
			for(var i = Models.Count-1; i >= 0; i--)
			{
				if(i >= SubStateMachines.Count) continue;
				Models[i] = SubStateMachines[i].Model;
			}
		}

		private void ReplaceModelsWithOriginalModels()
		{
			for(var i = Models.Count-1; i >= 0; i--)
			{
				if(Models[i] == null) continue;
				Models[i] = Models[i].Original;
			}
		}
	}
}