using System;
using Nonatomic.VSM2.NodeGraph;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class RandomIntegerState : State
	{
		[Transition(frameDelay: 0)] public event Action<int> OnContinueWithValue;
		
		[SerializeField] private int _min = 0;
		[SerializeField] private int _max = 1;

		[NonSerialized, Tooltip("Random value between min and max")] 
		public int RandomValue = 0;
		
		[Enter]
		public override void OnEnter()
		{
			RandomValue = Random.Range(_min, _max + 1);
			OnContinueWithValue?.Invoke(RandomValue);
		}

		[Enter]
		public void OnEnterWithMin(int value)
		{
			_min = value;
			OnEnter();
		}

		[Enter]
		public void OnEnterWithMax(int value)
		{
			_max = value;
			OnEnter();
		}
	}
}