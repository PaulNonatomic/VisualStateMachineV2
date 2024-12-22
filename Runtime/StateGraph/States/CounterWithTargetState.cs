using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph.States
{
	/**
	 * Remember that States~ are derived from ScriptableObject so
	 * it's important to flag that the Count value should not be
	 * serialized, although we reset the value on awake any way.
	 */
	[NodeColor(NodeColor.Dijon), NodeIcon(NodeIcon.PlusCircle)]
	public class CounterWithTargetState : BaseCounterState
	{
		[Transition(frameDelay:0)] public event Action OnContinue;
		[Transition(frameDelay:0)] public event Action<int> OnContinueWithCount;
		[Transition(frameDelay:0)] public event Action OnTargetReached;

		[SerializeField] private int _target = 2;
		[SerializeField] private bool _resetOnTarget = true;
		
		[Enter]
		public override void OnEnter()
		{
			Enter();
		}

		[Enter("Enter with target")]
		public void OnEnterWithTarget(int target)
		{
			_target = target;
			Enter();
		}

		private void Enter()
		{
			IncrementCounter();

			if (Count < _target)
			{
				OnContinue?.Invoke();
				OnContinueWithCount?.Invoke(Count);
			}
			else
			{
				if(_resetOnTarget) Count = 0;
				OnTargetReached?.Invoke();
			}
		}

		public override void OnExit()
		{
			//...
		}
	}
}