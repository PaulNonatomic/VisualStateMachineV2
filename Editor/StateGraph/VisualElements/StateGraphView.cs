﻿using System.Collections.Generic;
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
		private ToolBarView _toolBar;
		private FooterBarView _footerBar;
		private StateGraphContextMenu _contextMenu;

		public StateGraphView(string id) : base(id)
		{
			MakeToolBar();
			MakeFooterBar();
		}

		protected override void MakeStateManager(string id)
		{
			StateManager = new StateNodeGraphStateManager(id);
		}

		public override void PopulateGraph(NodeGraphDataModel model)
		{
			if(!model) return;
			
			var stateModel = model as StateMachineModel;
			ModelSelection.ActiveModel = model;
			
			if (StateManager.Model != stateModel)
			{
				EditorApplication.delayCall += HandleRecenter;
			}
			
			_toolBar.SetModel(stateModel);
			_footerBar.SetGridPosition(StateManager.GridPosition);
			_footerBar.SetModel(stateModel);

			base.PopulateGraph(model);
			AddEntryNode(stateModel);
			AddNodes(stateModel);
			AddEdges(stateModel);
		}
		
		private static void AddEntryNode(StateMachineModel stateModel)
		{
			if(!stateModel) return;
			if (stateModel.HasState<EntryState>()) return;
			StateGraphNodeFactory.MakeStateNodeData(stateModel, typeof(EntryState), Vector2.zero);
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
			
			EditorApplication.delayCall += () =>
			{
				PopulateGraph(model);
			};
		}

		private void AddNodes(StateMachineModel stateMachineModel)
		{
			var stateNodes = stateMachineModel.Nodes.Cast<StateNodeModel>();
			foreach (var nodeModel in stateNodes)
			{
				var nodeView = StateGraphNodeFactory.MakeNode(this, nodeModel, stateMachineModel); 
				AddElement(nodeView);
			}
		}

		private void AddEdges(StateMachineModel stateMachineModel)
		{
			foreach (var transition in stateMachineModel.Transitions)
			{
				StateGraphTransitionFactory.MakeTransitionView(this, transition);
			}
		}

		protected override void HandleAttachToPanel(AttachToPanelEvent evt)
		{
			base.HandleAttachToPanel(evt);
			
			EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
			OnGridPositionChanged += HandleGridPositionChanged;
			_toolBar.OnRecenter += HandleRecenter;
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
			_toolBar.OnRecenter -= HandleRecenter;
			_contextMenu.OnCreateNewStateNode -= HandleCreateNewStateNode;
			_contextMenu.OnDeleteEdgeContext -= HandleDeleteEdge;
			_contextMenu.OnDeleteStateNode -= HandleDeleteStateNode;
		}

		private void HandleRecenter()
		{
			var entryNode = this.Q<EntryNodeView>();
			if (entryNode == null) return;

			var entryNodePosition = entryNode.NodeModel.Position;
			var centerOffset = new Vector2(contentRect.width * 0.25f, 0);
			viewTransform.position = contentRect.center - entryNodePosition - centerOffset;
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			HandleGridPositionChanged(viewTransform.position);
		}

		private void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
		{
			var stateManager = (StateNodeGraphStateManager) StateManager;
			var gridPos = stateManager.GridPosition;
			
			switch (stateChange)
			{
				case PlayModeStateChange.EnteredPlayMode:
				case PlayModeStateChange.EnteredEditMode:
					stateManager.LoadModelFromStateController();
					PopulateGraph(StateManager.Model);
					break;
			}
			
			EditorApplication.delayCall += ()=>
			{
				SetGridPosition(gridPos);
			};
		}

		private void SetGridPosition(Vector2 gridPos)
		{
			viewTransform.position = contentRect.center + gridPos;
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			HandleGridPositionChanged(viewTransform.position);
		}

		protected override void HandleUpdate()
		{
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			_footerBar.SetGridPosition(StateManager.GridPosition);
			
			if (!Application.isPlaying) return;
			if (!StateManager.Model) return;
			
			var nodeViews = this.Query<BaseStateNodeView>().ToList();
			foreach (var nodeView in nodeViews)
			{
				nodeView.Update();
			}
		}

		private void HandleGridPositionChanged(Vector2 position)
		{
			_footerBar.SetGridPosition(StateManager.GridPosition);
		}

		private void HandleDeleteStateNode(NodeView nodeView)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			var nodesToDelete = new List<NodeView>() { nodeView };
			DeleteNodes(nodesToDelete);
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
			var screenPosition = Event.current != null 
				? GUIUtility.GUIToScreenPoint(Event.current.mousePosition) 
				: GUIUtility.ScreenToGUIPoint(MousePosition);
			
			StateSelectorWindow.Open(StateManager.Model, screenPosition, stateType =>
			{
				var model = (StateMachineModel) StateManager.Model;
				var nodeData = StateGraphNodeFactory.MakeStateNodeData(model, stateType, nodePosition);
				var nodeView = StateGraphNodeFactory.MakeNode(this, nodeData, model);
				AddElement(nodeView);
			});
		}

		private void MakeFooterBar()
		{
			_footerBar = new FooterBarView();
			Add(_footerBar);

			if (!StateManager.Model) return;
			_footerBar.SetModel(StateManager.Model as StateMachineModel);
		}

		private void MakeToolBar()
		{
			_toolBar = new ToolBarView();
			Insert(2, _toolBar);

			if (!StateManager.Model) return;
			_toolBar.SetModel(StateManager.Model as StateMachineModel);
		}

		protected override void HandleSelectionChanged()
		{
			HandleSelectionOfScriptableObject();
			HandleSelectionOfGameObject();
		}
		
		protected void HandleSelectionOfScriptableObject()
		{
			if (Selection.activeObject is not StateMachineModel model) return;
			
			ModelSelection.ActiveModel = model;
		}

		protected void HandleSelectionOfGameObject()
		{
			if (!Selection.activeGameObject) return;
			if (!Selection.activeGameObject.TryGetComponent(out StateMachineController stateMachineController)) return;
			
			ModelSelection.ActiveModel = stateMachineController.Model;
			
			var stateManager = (StateNodeGraphStateManager) StateManager;
			stateManager.SetStateControllerId(stateMachineController.Id);
		}
	}
}