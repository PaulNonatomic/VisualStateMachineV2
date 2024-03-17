using System;
using Nonatomic.VSM2.NodeGraph;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	
	public class ParallelSubStateMachineState : BaseParallelSubStateMachineState
	{
		[Transition]
		public event Action OnComplete;
		
		[SerializeField] protected ParallelCompletionMode CompletionMode = ParallelCompletionMode.Any;

		private int _completionCount;

		public override void OnEnterState()
		{
			_completionCount = 0;
			
			base.OnEnterState();
		}
		
		protected override void OnSubStateComplete(State state, StateMachineModel model)
		{
			switch (CompletionMode)
			{
				case ParallelCompletionMode.Any:
					CompletionAnyMode();
					break;
				case ParallelCompletionMode.All:
					CompletionAllMode();
					break;
			}
			
			base.OnSubStateComplete(state, model);
		}

		private void CompletionAllMode()
		{
			_completionCount++;
			if (_completionCount != Models.Count) return;
			
			OnComplete?.Invoke();
		}

		private void CompletionAnyMode()
		{
			OnComplete?.Invoke();
		}
	}
}