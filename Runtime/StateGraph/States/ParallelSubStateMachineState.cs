using System;
using Nonatomic.VSM2.NodeGraph;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class ParallelSubStateMachineState : BaseParallelSubStateMachineState
	{
		[Transition]
		public event Action OnComplete;

		private int _completionCount;

		public override void OnEnterState()
		{
			_completionCount = 0;
			
			base.OnEnterState();
		}
		
		protected override void HandleComplete(State state)
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