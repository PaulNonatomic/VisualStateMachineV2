using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;

namespace Samples.Animation.Source.States
{
	public class MoveState : State
	{
		[Transition] 
		public event Action OnLeftClick;

		[Transition] 
		public event Action OnRightClick;
		
		[SerializeField] private float _moveSpeed = 1.0f;
		[SerializeField] private float _maxHeight = 2.0f;
		[SerializeField] private float _minHeight = 0.5f;
		
		private AnimationController _animController;
		private float _time;
		private Vector3 _basePosition;

		public override void OnEnter()
		{
			_animController = GameObject.GetComponent<AnimationController>();
			_animController.OnLeftClick += HandleLeftClick;
			_animController.OnRightClick += HandleRightClick;
			_basePosition = _animController.LocalPosition;
		}

		public override void OnUpdate()
		{
			_time += Time.deltaTime * _moveSpeed;
			var sin = Mathf.Sin(_time);
			var height = (sin + 1) / 2 * (_maxHeight - _minHeight) + _minHeight;
			var position = _basePosition;
			position.y = height;
			
			_animController.MoveCube(position, Space.Self);
		}

		public override void OnExit()
		{
			_animController.OnLeftClick -= HandleLeftClick;
			_animController.OnRightClick -= HandleRightClick;
		}

		private void HandleLeftClick() => OnLeftClick?.Invoke();
		private void HandleRightClick() => OnRightClick?.Invoke();
	}
}