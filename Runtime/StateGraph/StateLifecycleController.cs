using System.Collections.Generic;
using Nonatomic.VSM2.Data;
using Nonatomic.VSM2.NodeGraph;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	/// <summary>
	///     Manages the lifecycle of states in the state machine
	/// </summary>
	public class StateLifecycleController
	{
		private readonly ISharedData _sharedData;
		private StateNodeModel _currentNode;

		public StateLifecycleController(ISharedData sharedData)
		{
			_sharedData = sharedData;
		}

		public void SetCurrentNode(StateNodeModel node)
		{
			_currentNode = node;
		}

		public StateNodeModel GetCurrentNode()
		{
			return _currentNode;
		}

		public void Update()
		{
			if (_currentNode == null)
			{
				Debug.LogWarning("StateLifecycleController: currentNode is null in Update");
				return;
			}
    
			Debug.Log($"StateLifecycleController.Update for node {_currentNode.Id}, Active: {_currentNode.Active}");
    
			if (!_currentNode.Active)
			{
				Debug.LogWarning($"Node {_currentNode.Id} is not Active, skipping Update");
				return;
			}
    
			_currentNode.Update();
		}

		public void FixedUpdate()
		{
			if (_currentNode == null) return;
			if (!_currentNode.Active) return;

			_currentNode.FixedUpdate();
		}

		public void LateUpdate()
		{
			if (_currentNode == null) return;
			if (!_currentNode.Active) return;

			_currentNode.LateUpdate();
		}

		public void Start(List<StateNodeModel> nodes)
		{
			foreach (var node in nodes)
			{
				if (!node.Enabled) continue;
				node.Start();
			}
		}

		public void Enter(StateNodeModel entryNode)
		{
			_currentNode = entryNode;
			Debug.Log($"StateLifecycleController {_currentNode.State.name}.Enter()");
			_currentNode.Enter(TransitionEventData.Empty);
		}

		public void Exit()
		{
			if (_currentNode == null) return;
			Debug.Log($"StateLifecycleController {_currentNode.State.name}.Exit()");
			_currentNode.Exit();
		}

		public void Destroy(List<StateNodeModel> nodes)
		{
			Debug.Log($"StateLifecycleController {_currentNode.State.name}.Exit()");
			if (_currentNode != null) _currentNode.Exit();
			foreach (var node in nodes) node?.OnDestroy();

			_sharedData.ClearAllData();
		}
	}
}