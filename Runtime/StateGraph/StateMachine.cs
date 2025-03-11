using System;
using System.Collections.Generic;
using Nonatomic.VSM2.Data;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEngine;

namespace Nonatomic.VSM2.StateGraph
{
	public class StateMachine
	{
		private readonly EventSubscriptionService _eventSubscriptionService;
		private readonly Dictionary<JumpId, StateNodeModel> _jumpNodeLookup = new();

		private readonly StateLifecycleController _lifecycleController;

		private readonly Dictionary<string, StateNodeModel> _nodeLookup = new();
		private readonly Dictionary<string, List<StateTransitionModel>> _transitionLookup = new();
		private readonly StateTransitionManager _transitionManager;

		public StateMachine(StateMachineModel model, StateMachineController controller)
			: this(model, controller, null)
		{
			// ...
		}

		public StateMachine(StateMachineModel model, StateMachineController controller, ISharedData sharedData = null)
		{
			SharedData = sharedData ?? new SharedData();
    
			// Initialize services
			_lifecycleController = new StateLifecycleController(SharedData);
			_eventSubscriptionService = new EventSubscriptionService(
				_transitionLookup, 
				HandleTransition);
			_transitionManager = new StateTransitionManager(
				_nodeLookup, 
				_transitionLookup, 
				_jumpNodeLookup, 
				_eventSubscriptionService,
				OnNodeChanged);
    
			Initialize(model, controller);
		}

		public ISharedData SharedData { get; }

		public State State => _lifecycleController.GetCurrentNode()?.State;
		public StateMachineModel Model { get; private set; }
		public bool IsComplete { get; private set; }
		public event Action<State, StateMachineModel> OnComplete;

		public void Update()
		{
			_lifecycleController.Update();
		}

		public void FixedUpdate()
		{
			_lifecycleController.FixedUpdate();
		}

		public void LateUpdate()
		{
			_lifecycleController.LateUpdate();
		}

		public void Start()
		{
			_lifecycleController.Start(Model.Nodes);
		}

		public void Enter()
		{
			Debug.Log("StateMachine.Enter");
			if (Model.TryGetNodeByState<EntryState>(out var entryNode))
			{
				_lifecycleController.SetCurrentNode(entryNode);
				_transitionManager.SetCurrentNode(entryNode);
			}

			IsComplete = false;
			_eventSubscriptionService.SubscribeToNode(_lifecycleController.GetCurrentNode());

			_lifecycleController.Enter(_lifecycleController.GetCurrentNode());
		}

		public void Exit()
		{
			_lifecycleController.Exit();
			IsComplete = true;
		}

		public void JumpTo(JumpId jumpId)
		{
			_transitionManager.JumpTo(jumpId);
		}

		public void OnDestroy()
		{
			_transitionManager?.CancelAllTransitions();
			_lifecycleController?.Destroy(Model != null ? Model.Nodes : new List<StateNodeModel>());

			Model = null;
		}
		
		private void OnNodeChanged(StateNodeModel newNode)
		{
			_lifecycleController.SetCurrentNode(newNode);
		}

		private void Initialize(StateMachineModel model, StateMachineController controller)
		{
			Model = StateMachineModel.CreateInstance(model);
			Model.Initialize(controller, this, SharedData);

			CreateNodeLookupTable();
			CreateTransitionLookupTable();
		}

		private void CreateTransitionLookupTable()
		{
			_transitionLookup.Clear();

			foreach (var transition in Model.Transitions)
			{
				if (!_transitionLookup.ContainsKey(transition.OriginNodeId)) _transitionLookup.Add(transition.OriginNodeId, new List<StateTransitionModel>());

				_transitionLookup[transition.OriginNodeId].Add(transition);
			}
		}

		private void CreateNodeLookupTable()
		{
			_nodeLookup.Clear();
			_jumpNodeLookup.Clear();

			foreach (var node in Model.Nodes)
			{
				if (!node.State) continue;
				_nodeLookup.Add(node.Id, node);

				if (node.State is not JumpInState jumpIn) continue;
				_jumpNodeLookup.Add(jumpIn.JumpId, node);
			}
		}

		private void HandleTransition(TransitionModel transition, TransitionEventData eventData)
		{
			Debug.Log($"StateMachine.HandleTransition: {transition.OriginNodeId} -> {transition.DestinationNodeId}");
			_transitionManager.HandleTransition(transition, eventData);
		}

		public void Complete(State state)
		{
			if (IsComplete) return;

			Debug.Log($"StateMachine.Complete: {state.name}");
			IsComplete = true;
			OnComplete?.Invoke(state, Model);
		}

		public void SetParent(StateMachine stateMachine)
		{
			Model.SetParent(stateMachine.Model);
		}
	}
}