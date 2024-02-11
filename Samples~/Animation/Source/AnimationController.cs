using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Samples.Animation.Source
{
	public class AnimationController : MonoBehaviour
	{
		public event Action OnLeftClick;
		public event Action OnRightClick;

		public Vector3 Scale => _cube.localScale;
		public Vector3 Position => _cube.position;
		public Vector3 LocalPosition => _cube.localPosition;
		
		[SerializeField] private Transform _cube;
		
		private RaycastHit _hit;
		private Camera _camera;

		private void Start()
		{
			_camera = Camera.main;
			Assert.IsNotNull(_camera);
		}

		public void Awake()
		{
			Assert.IsNotNull(_cube);
		}
		
		public void RotateCube(Vector3 axis, float angle, Space space)
		{
			_cube.Rotate(axis, angle, space);
		}
		
		public void ScaleCube(Vector3 scale)
		{
			_cube.localScale = scale;
		}

		public void MoveCube(Vector3 position, Space space)
		{
			switch (space)
			{
				case Space.World:
					_cube.position = position;
					break;
				case Space.Self:
					_cube.localPosition = position;
					break;
			}
		}

		public void Update()
		{
			CheckForClick(0);
			CheckForClick(1);
		}

		private void CheckForClick(int button)
		{
			if (!Input.GetMouseButtonDown(button)) return;
			
			var ray = _camera.ScreenPointToRay(Input.mousePosition);
			
			if (!Physics.Raycast(ray, out _hit)) return;
			if (_hit.transform != _cube) return;
			
			switch (button)
			{
				case 0:
					OnLeftClick?.Invoke();
					break;
				case 1:
					OnRightClick?.Invoke();
					break;
			}
		}
	}
}