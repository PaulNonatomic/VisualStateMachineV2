using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using Nonatomic.VSM2.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateGraphView : NodeGraphView
	{
		private TitleBarView _titleBar;
		private StateGraphContextMenu _contextMenu;

		public StateGraphView(string id) : base(id)
		{
			MakeTitleBar();
		}
		
		protected override void MakeStateManager(string id)
		{
			StateManager = new StateNodeGraphStateManager(id);
		}

		public override void PopulateGraph(NodeGraphDataModel model)
		{
			if (model == null) return;
			if (model != StateManager.Model)
			{
				HandleRecenter();
			}
			
			base.PopulateGraph(model);

			_titleBar.SetTitle(model.name);
			_titleBar.SetGridPosition(StateManager.GridPosition);

			var stateModel = model as StateMachineModel;
			
			AddEntryNode(stateModel);
			AddNodes(stateModel);
			AddEdges(stateModel);
		}

		private void AddEntryNode(StateMachineModel stateModel)
		{
			if (stateModel.HasState<EntryState>()) return;
			StateGraphFactory.MakeStateNodeData(stateModel, typeof(EntryState), Vector2.zero);
		}

		public override EventPropagation DeleteSelection()
		{
			var selectedEdges = selection.OfType<Edge>().ToList();
			var selectedStateNodes = selection.OfType<NodeView>().ToList();
			
			DeleteEdges(selectedEdges);
			DeleteNodes(selectedStateNodes);
			
			return base.DeleteSelection();
		}

		private void DeleteEdges(List<Edge> edges)
		{
			var model = (StateMachineModel) StateManager.Model;
			foreach (var edge in edges)
			{
				var transitionData = edge.userData as StateTransitionModel;
				model.RemoveTransition(transitionData);
			}

			PopulateGraph(model);
		}

		private void DeleteNodes(List<NodeView> stateNodeViews)
		{
			var model = (StateMachineModel) StateManager.Model;
			foreach (var node in stateNodeViews)
			{
				var nodeData = node.userData as StateNodeModel;
				model.RemoveState(nodeData);
			}
			
			PopulateGraph(model);
		}

		private void AddNodes(StateMachineModel stateMachineModel)
		{
			var stateNodes = stateMachineModel.Nodes.Cast<StateNodeModel>();
			foreach (var nodeModel in stateNodes)
			{
				StateGraphFactory.MakeNode(this, nodeModel, stateMachineModel);
			}
		}
		
		private void AddEdges(StateMachineModel stateMachineModel)
		{
			foreach (var transition in stateMachineModel.Transitions)
			{
				StateGraphFactory.MakeTransitionView(this, transition);
			}
		}

		protected override void HandleAttachToPanel(AttachToPanelEvent evt)
		{
			base.HandleAttachToPanel(evt);
			
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			OnGridPositionChanged += HandleGridPositionChanged;
			_titleBar.OnRecenter += HandleRecenter;
			_contextMenu = new StateGraphContextMenu(this);
			_contextMenu.OnCreateNewStateNode += HandleCreateNewStateNode;
			_contextMenu.OnDeleteEdgeContext += HandleDeleteEdge;
			_contextMenu.OnDeleteStateNode += HandleDeleteStateNode;
		}

		protected override void HandleLeavePanel(DetachFromPanelEvent evt)
		{
			base.HandleLeavePanel(evt);
			
			EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
			OnGridPositionChanged -= HandleGridPositionChanged;
			_titleBar.OnRecenter += HandleRecenter;
			_contextMenu.OnCreateNewStateNode -= HandleCreateNewStateNode;
			_contextMenu.OnDeleteEdgeContext -= HandleDeleteEdge;
			_contextMenu.OnDeleteStateNode -= HandleDeleteStateNode;
		}

		private void HandleRecenter()
		{
			var entryNode = this.Q<EntryNodeView>();
			if (entryNode == null) return;

			var centerOffset = new Vector2(contentRect.width * 0.25f, 0);
			viewTransform.position = contentRect.center - entryNode.GetPosition().center - centerOffset;
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			HandleGridPositionChanged(viewTransform.position);
		}

		private void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
		{
			var stateManager = (StateNodeGraphStateManager) StateManager;
			
			switch (stateChange)
			{
				case PlayModeStateChange.EnteredPlayMode:
				case PlayModeStateChange.EnteredEditMode:
					stateManager.LoadModelFromStateController();
					PopulateGraph(StateManager.Model);
					break;
			}
		}
		
		protected override void HandleUpdate()
		{
			if (!Application.isPlaying) return;
			if (StateManager.Model == null) return;

			var nodeViews = this.Query<BaseStateNodeView>().ToList();
			foreach (var nodeView in nodeViews)
			{
				nodeView.Update();
			}
		}

		private void HandleGridPositionChanged(Vector2 position)
		{
			_titleBar.SetGridPosition(StateManager.GridPosition);
		}

		private void HandleDeleteStateNode(NodeView nodeView)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			DeleteNodes(new List<NodeView>(){nodeView});
		}

		private void HandleDeleteEdge(StateNodeEdge edge)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			DeleteEdges(new List<Edge>(){edge});
		}

		private void HandleCreateNewStateNode(Vector2 position)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
			
			var nodePosition = GraphUtils.ScreenPointToGraphPoint(position, this);
			var mousePosition = Event.current.mousePosition;
			var screenPosition = GUIUtility.GUIToScreenPoint(mousePosition);
			
			StateSelectorWindow.Open(StateManager.Model, screenPosition, stateType =>
			{
				var model = (StateMachineModel) StateManager.Model;
				var nodeData = StateGraphFactory.MakeStateNodeData(model, stateType, nodePosition);
				StateGraphFactory.MakeNode(this, nodeData, model);
			});
		}

		private void MakeTitleBar()
		{
			_titleBar = new TitleBarView();

			if (StateManager.Model != null)
			{
				_titleBar.SetTitle(StateManager.Model.name);
			}
			
			Insert(2, _titleBar);
		}

		protected override void HandleSelectionChanged()
		{
			switch (Selection.activeObject)
			{
				case StateMachineModel:
					PopulateGraph(Selection.activeObject as StateMachineModel);
					break;
				case GameObject:
					var go = Selection.activeObject as GameObject;
					var smc = go.GetComponent<StateMachineController>();
					if (smc == null) break;

					var model = smc.Model;
					if (model == null) break;
					
					var stateManager = StateManager as StateNodeGraphStateManager;
					stateManager.SetStateControllerId(smc.Id);
					
					PopulateGraph(model);
					break;
			}
		}
	}
}