using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;

namespace Samples.Animation.Source.States
{
	public class RotateState : State
	{
		[Transition] 
		public event Action OnLeftClick;

		[Transition] 
		public event Action OnRightClick;
		
		[SerializeField] private Vector3 _axis;
		[SerializeField] private float _angle;
		[SerializeField] private Space _space;
		
		private AnimationController _animController;
		private float _time;

		public override void Enter()
		{
			_time = Time.time;
			_animController = GameObject.GetComponent<AnimationController>();
			_animController.OnLeftClick += HandleLeftClick;
			_animController.OnRightClick += HandleRightClick;
		}

		public override void Update()
		{
			_animController.RotateCube(_axis, _angle * Time.deltaTime, _space);
		}

		public override void Exit()
		{
			_animController.OnLeftClick -= HandleLeftClick;
			_animController.OnRightClick -= HandleRightClick;
		}

		private void HandleLeftClick() => OnLeftClick?.Invoke();
		private void HandleRightClick() => OnRightClick?.Invoke();
	}
}