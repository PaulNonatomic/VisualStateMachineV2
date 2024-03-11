using System;
using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.NodeGraph
{
	public class NodeGraphView : GraphView
	{
		public event Action<Vector2> OnGridPositionChanged;
		
		protected NodeGraphStateManager StateManager;
		protected Vector2 Size;
		protected Vector2 MousePosition;

		public NodeGraphView(string id)
		{
			MakeStateManager(id);
			MakeGrid();
			MakeGridShadow();
			AddManipulators();
			
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
			RegisterCallback<DetachFromPanelEvent>(HandleLeavePanel);
			RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);
			RegisterCallback<MouseMoveEvent>(HandleMouseMove);
		}

		public virtual void PopulateGraph(NodeGraphDataModel model)
		{
			if (model == null) return;
			
			StateManager.SetModel(model);
			ClearGraph();
		}

		protected virtual void MakeStateManager(string id)
		{
			StateManager = new NodeGraphStateManager(id);
		}

		protected virtual void HandleMouseMove(MouseMoveEvent evt)
		{
			MousePosition = evt.localMousePosition;
		}

		/**
		 * This method is responsible for allowing specific ports to connect to each other.
		 */
		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			if (Application.isPlaying) return compatiblePorts;
			
			foreach (var port in ports)
			{
				if (ElementUtils.BothContainClass(port, startPort, "output")) continue;
				if (ElementUtils.BothContainClass(port, startPort, "input")) continue;

				if (startPort == port || startPort.node == port.node) continue;
				if (startPort.capacity == Port.Capacity.Single && startPort.connections.ToList().Count > 0) continue;
				if (port.capacity == Port.Capacity.Single && port.connections.ToList().Count > 0) continue;
				
				compatiblePorts.Add(port);
			}
			
			return compatiblePorts;
		}

		protected virtual void HandleStateChange()
		{
			EditorApplication.delayCall += () =>
			{
				PopulateGraph(StateManager.Model);
			};
		}

		protected virtual void HandleAttachToPanel(AttachToPanelEvent evt)
		{
			StateManager.OnChange += HandleStateChange;
			Selection.selectionChanged += HandleSelectionChanged;
			ModelSelection.OnModelSelected += HandleModelChanged;
			EditorApplication.update += HandleUpdate;
			viewTransformChanged += HandleViewTransformChanged;

			EditorApplication.delayCall += () =>
			{
				viewTransform.position = contentRect.center + StateManager.GridPosition;
				viewTransform.scale = StateManager.GridScale;
			};
			
			StateManager.LoadState();
		}

		protected virtual void HandleLeavePanel(DetachFromPanelEvent evt)
		{
			SaveState();
			
			StateManager.OnChange -= HandleStateChange;
			Selection.selectionChanged -= HandleSelectionChanged;
			ModelSelection.OnModelSelected -= HandleModelChanged;
			EditorApplication.update -= HandleUpdate;
		}

		protected virtual void HandleUpdate()
		{
			if (StateManager.Model == null) return;
		}

		protected virtual void HandleModelChanged(NodeGraphDataModel model)
		{
			PopulateGraph(model);
		}

		protected virtual void ClearGraph()
		{
			DeleteElements(graphElements);
		}

		protected virtual void MakeGrid()
		{
			var grid = new GridBackground();
			grid.name = "grid";
			Insert(0, grid);
		}

		protected virtual void AddManipulators()
		{
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());
		}

		private void HandleGeometryChanged(GeometryChangedEvent evt)
		{
			Size = evt.newRect.size;
			StateManager.LoadState();
		}

		private void MakeGridShadow()
		{
			var shadow = new VisualElement();
			shadow.name = "gridShadow";
			shadow.style.backgroundImage= Resources.Load<Texture2D>("DropShadowSliced2");
			shadow.pickingMode = PickingMode.Ignore;
			
			Add(shadow);
		}

		private void HandleViewTransformChanged(GraphView graphView)
		{
			SaveState();
		}

		private void SaveState()
		{
			StateManager.SetGridPosition(contentRect.center, viewTransform.position);
			StateManager.SetGridScale(viewTransform.scale);
			
			OnGridPositionChanged?.Invoke(viewTransform.position);
		}

		protected virtual void HandleSelectionChanged()
		{
			//..
		}
	}
}