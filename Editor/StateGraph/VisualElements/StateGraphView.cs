// Main graph view class with reduced responsibilities

using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using Nonatomic.VSM2.Editor.StateGraph.Nodes.Base;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.VisualElements
{
	public class StateGraphView : NodeGraphView
	{
		private readonly float _creationTime;
		private GraphClipboardService _clipboardService;
		private StateGraphContextMenu _contextMenu;
		private GraphEventHandler _eventHandler;
		private FooterBarView _footerBar;
		private GraphPopulationService _populationService;
		private GraphSelectionHandler _selectionHandler;
		private ToolBarView _toolBar;

		public StateGraphView(string id) : base(id)
		{
			_creationTime = Time.time;

			// Initialize services
			_populationService = new GraphPopulationService(this);
			_selectionHandler = new GraphSelectionHandler(this);
			_clipboardService = new GraphClipboardService(this);
			_eventHandler = new GraphEventHandler(this);

			// Initialize UI components
			MakeToolBar();
			MakeFooterBar();

			// Set up event handlers
			RegisterCallback<KeyDownEvent>(_eventHandler.OnKeyDown);
		}

		public void PopulateGraph(NodeGraphDataModel model, bool recentre)
		{
			if (!model) return;

			var stateModel = model as StateMachineModel;
			var activeTime = Time.time - _creationTime;
			ModelSelection.ActiveModel = model;

			// Update UI components
			_toolBar.SetModel(stateModel);
			_footerBar.SetGridPosition(StateManager.GridPosition);
			_footerBar.SetModel(stateModel);

			// Defer to base class for basic population
			base.PopulateGraph(model);

			// Use the specialized population service for state machine specific elements
			_populationService.AddEntryNode(stateModel);
			_populationService.PopulateWithDelay(stateModel, recentre);
		}

		public override void PopulateGraph(NodeGraphDataModel model)
		{
			PopulateGraph(model, true);
		}

		protected override void MakeStateManager(string id)
		{
			StateManager = new StateNodeGraphStateManager(id);
		}

		public override EventPropagation DeleteSelection()
		{
			_selectionHandler.DeleteSelectedElements();
			return base.DeleteSelection();
		}
		
		public void HandleRecenter()
		{
			var entryNode = this.Q<EntryNodeView>();
			if (entryNode == null) return;

			var entryNodePosition = entryNode.NodeModel.Position;
			var centerOffset = new Vector2(contentRect.width * 0.25f, 0);
			viewTransform.position = contentRect.center - entryNodePosition - centerOffset;
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			_footerBar.SetGridPosition(StateManager.GridPosition);
		}

		protected override void HandleAttachToPanel(AttachToPanelEvent evt)
		{
			base.HandleAttachToPanel(evt);

			// Register event handlers
			EditorApplication.playModeStateChanged += _eventHandler.HandlePlayModeStateChanged;
			OnGridPositionChanged += _eventHandler.HandleGridPositionChanged;

			// Set up toolbar events
			_toolBar.OnRecenter += _eventHandler.HandleRecenter;
			_toolBar.OnSave += _eventHandler.HandleSave;

			// Set up context menu
			_contextMenu = new StateGraphContextMenu(this);
			_contextMenu.OnCreateNewStateNode += _eventHandler.HandleCreateNewStateNode;
			_contextMenu.OnCreateNewStickyNote += _eventHandler.HandleCreateNewStickyNote;
			_contextMenu.OnDeleteEdgeContext += _eventHandler.HandleDeleteEdge;
			_contextMenu.OnDeleteStateNode += _eventHandler.HandleDeleteStateNode;
			_contextMenu.OnDeleteSelection += _eventHandler.HandleDeleteSelection;
			_contextMenu.OnCopySelected += _clipboardService.HandleCopySelected;
			_contextMenu.OnPasteSelected += _clipboardService.HandlePasteSelected;
		}

		protected override void HandleLeavePanel(DetachFromPanelEvent evt)
		{
			base.HandleLeavePanel(evt);

			// Unregister event handlers
			EditorApplication.playModeStateChanged -= _eventHandler.HandlePlayModeStateChanged;
			OnGridPositionChanged -= _eventHandler.HandleGridPositionChanged;

			// Unset toolbar events
			_toolBar.OnRecenter -= _eventHandler.HandleRecenter;
			_toolBar.OnSave -= _eventHandler.HandleSave;

			// Unset context menu events
			_contextMenu.OnCreateNewStateNode -= _eventHandler.HandleCreateNewStateNode;
			_contextMenu.OnCreateNewStickyNote -= _eventHandler.HandleCreateNewStickyNote;
			_contextMenu.OnDeleteEdgeContext -= _eventHandler.HandleDeleteEdge;
			_contextMenu.OnDeleteSelection -= _eventHandler.HandleDeleteSelection;
			_contextMenu.OnDeleteStateNode -= _eventHandler.HandleDeleteStateNode;
			_contextMenu.OnCopySelected -= _clipboardService.HandleCopySelected;
			_contextMenu.OnPasteSelected -= _clipboardService.HandlePasteSelected;
		}

		protected override void HandleUpdate()
		{
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			_footerBar.SetGridPosition(StateManager.GridPosition);

			if (!Application.isPlaying) return;
			if (!StateManager.Model) return;

			// Update all node views
			var nodeViews = this.Query<BaseStateNodeView>().ToList();
			foreach (var nodeView in nodeViews) nodeView.Update();
		}

		protected override void HandleSelectionChanged()
		{
			_selectionHandler.HandleSelectionChanged();
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
	}
}