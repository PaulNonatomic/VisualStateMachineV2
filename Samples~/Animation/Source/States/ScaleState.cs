using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEngine;

namespace Samples.Animation.Source.States
{
	public class ScaleState : State
	{
		[Transition] 
		public event Action LeftClick;

		[Transition] 
		public event Action RightClick;
		
		[SerializeField] private float _scaleSpeed = 1.0f;
		[SerializeField] private float _maxScale = 2.0f;
		[SerializeField] private float _minScale = 0.5f;
		
		private AnimationController _animController;
		private float _time;
		private Vector3 _baseScale;

		public override void OnEnter()
		{
			_animController = GameObject.GetComponent<AnimationController>();
			_animController.OnLeftClick += HandleLeftClick;
			_animController.OnRightClick += HandleRightClick;
			_baseScale = _animController.Scale;
		}

		public override void OnUpdate()
		{
			_time += Time.deltaTime * _scaleSpeed;
			var sin = Mathf.Sin(_time);
			var scaleFactor = (sin + 1) / 2 * (_maxScale - _minScale) + _minScale;
			
			_animController.ScaleCube(_baseScale * scaleFactor);
		}

		public override void OnExit()
		{
			_animController.OnLeftClick -= HandleLeftClick;
			_animController.OnRightClick -= HandleRightClick;
		}

		private void HandleLeftClick() => LeftClick?.Invoke();
		private void HandleRightClick() => RightClick?.Invoke();
	}
}