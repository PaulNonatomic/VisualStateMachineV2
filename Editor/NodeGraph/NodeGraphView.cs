using System;
using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.NodeGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.NodeGraph
{
	public class NodeGraphView : GraphView
	{
		public event Action<Vector2> OnGridPositionChanged;
		public NodeGraphStateManager StateManager;
		
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
			StateManager.SetModel(model);
			ClearGraph();
		}

		protected virtual void MakeStateManager(string id)
		{
			StateManager = new NodeGraphStateManager(id);
		}

		private void HandleMouseMove(MouseMoveEvent evt)
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
			
			var startPortModel = startPort.userData as PortModel;
			if (startPortModel == null) return compatiblePorts;
			
			foreach (var port in ports)
			{
				if (ElementUtils.BothContainClass(port, startPort, "output")) continue;
				if (ElementUtils.BothContainClass(port, startPort, "input")) continue;

				if (startPort == port || startPort.node == port.node) continue;
				if (startPort.capacity == Port.Capacity.Single && startPort.connections.ToList().Count > 0) continue;
				if (port.capacity == Port.Capacity.Single && port.connections.ToList().Count > 0) continue;

				var endPortModel = port.userData as PortModel;
				if(endPortModel == null) continue;
				if(startPortModel.PortTypeName != endPortModel.PortTypeName) continue;
				
				compatiblePorts.Add(port);
			}
			
			return compatiblePorts;
		}

		private void HandleStateChange()
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
			if (!StateManager.Model) return;
		}

		private void HandleModelChanged(NodeGraphDataModel model)
		{
			PopulateGraph(model);
		}

		protected virtual void ClearGraph()
		{
			DeleteElements(graphElements);
		}

		private void MakeGrid()
		{
			var grid = new GridBackground();
			grid.name = "grid";
			Insert(0, grid);
		}

		private void AddManipulators()
		{
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			this.AddManipulator(new FreehandSelector());
		}

		private void HandleGeometryChanged(GeometryChangedEvent evt)
		{
			StateManager.LoadState();
		}

		private void MakeGridShadow()
		{
			var shadow = new VisualElement
			{
				name = "gridShadow",
				style =
				{
					backgroundImage = Resources.Load<Texture2D>("DropShadowSliced2")
				},
				pickingMode = PickingMode.Ignore
			};

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