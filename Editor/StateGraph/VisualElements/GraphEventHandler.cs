using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Persistence;
using Nonatomic.VSM2.Editor.StateGraph.Factories;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using Nonatomic.VSM2.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.VisualElements
{
	public class GraphEventHandler
	{
		private readonly StateGraphView _graphView;

		public GraphEventHandler(StateGraphView graphView)
		{
			_graphView = graphView;
		}

		public void OnKeyDown(KeyDownEvent evt)
		{
			HandleCopyAndPasteKeys(evt);
		}

		public void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
		{
			Debug.Log($"GraphEventHandler.HandlePlayModeStateChanged: {stateChange}");
			
			var stateManager = (StateNodeGraphStateManager)_graphView.StateManager;
			var gridPos = stateManager.GridPosition;

			stateManager.LoadModelFromStateController();
			_graphView.PopulateGraph(stateManager.Model, true);

			EditorApplication.delayCall += () => { SetGridPosition(gridPos); };
		}

		public void HandleGridPositionChanged(Vector2 position)
		{
			var footerBar = _graphView.Q<FooterBarView>();
			if (footerBar != null) footerBar.SetGridPosition(_graphView.StateManager.GridPosition);
		}

		public void HandleRecenter()
		{
			_graphView.HandleRecenter();
		}

		public void HandleSave()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			var model = (StateMachineModel)_graphView.StateManager.Model;
			model = StateMachineModelUtils.UpdatePortDataInModel(model);

			StateMachineModelSaver.Save(model);
		}

		public void HandleDeleteSelection()
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			_graphView.DeleteSelection();
		}

		public void HandleDeleteStateNode(NodeView nodeView)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			var selectionHandler = new GraphSelectionHandler(_graphView);
			selectionHandler.DeleteSelectedElements();
		}

		public void HandleDeleteEdge(StateNodeEdge edge)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			var model = (StateMachineModel)_graphView.StateManager.Model;
			var transitionData = edge.userData as StateTransitionModel;
			model.RemoveTransition(transitionData);

			Debug.Log($"GraphEventHandler.HandleDeleteEdge");
			EditorApplication.delayCall += () => { _graphView.PopulateGraph(model, false); };
		}

		public void HandleCreateNewStickyNote(Vector2 position)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;

			CreateNewStateNode(position, typeof(StickyNoteState));
		}

		public void HandleCreateNewStateNode(Vector2 position)
		{
			if (GuardUtils.GuardAgainstRuntimeOperation()) return;
    
			var screenPosition = Event.current != null 
				? GUIUtility.GUIToScreenPoint(Event.current.mousePosition) 
				: GUIUtility.ScreenToGUIPoint(position);
    
			StateSelectorWindow.Open(_graphView.StateManager.Model, screenPosition, stateType =>
			{
				CreateNewStateNode(position, stateType);
			});
		}

		private void CreateNewStateNode(Vector2 position, Type stateType)
		{
			var nodePosition = GraphUtils.ScreenPointToGraphPointWithZoom(position, _graphView);
			var model = (StateMachineModel)_graphView.StateManager.Model;
			var nodeData = StateGraphNodeFactory.MakeStateNodeData(model, stateType, nodePosition);
			var nodeView = StateGraphNodeFactory.MakeNode(_graphView, nodeData, model);
			_graphView.AddElement(nodeView);
		}

		private void HandleCopyAndPasteKeys(KeyDownEvent evt)
		{
			if (!evt.ctrlKey && !evt.commandKey) return;

			switch (evt.keyCode)
			{
				case KeyCode.C:
					var clipboardService = new GraphClipboardService(_graphView);
					clipboardService.HandleCopySelected();
					evt.StopPropagation();
					break;
				case KeyCode.V:
					var pasteService = new GraphClipboardService(_graphView);
					pasteService.HandlePasteSelected();
					evt.StopPropagation();
					break;
			}
		}

		private void SetGridPosition(Vector2 gridPos)
		{
			_graphView.viewTransform.position = _graphView.contentRect.center + gridPos;
			_graphView.StateManager.SetGridPosition(_graphView.contentRect.center, _graphView.viewTransform.position);
			HandleGridPositionChanged(_graphView.viewTransform.position);
		}
	}
}