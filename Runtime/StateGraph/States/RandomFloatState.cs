using System;
using Nonatomic.VSM2.NodeGraph;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nonatomic.VSM2.StateGraph.States
{
	public class RandomFloatState : State
	{
		[Transition(frameDelay: 0)] public event Action<float> OnContinueWithValue;
		
		[SerializeField] private float _min = 0f;
		[SerializeField] private float _max = 1f;

		[NonSerialized, Tooltip("Random value between min and max")] 
		public float RandomValue = 0;
		
		[Enter]
		public override void OnEnter()
		{
			RandomValue = Random.Range(_min, _max);
			OnContinueWithValue?.Invoke(RandomValue);
		}

		[Enter]
		public void OnEnterWithMin(float value)
		{
			_min = value;
			OnEnter();
		}

		[Enter]
		public void OnEnterWithMax(float value)
		{
			_max = value;
			OnEnter();
		}
	}
}