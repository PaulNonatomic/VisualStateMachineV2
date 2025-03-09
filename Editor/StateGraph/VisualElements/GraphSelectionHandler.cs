using System.Collections.Generic;
using System.Linq;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Nonatomic.VSM2.Editor.StateGraph.VisualElements
{
	public class GraphSelectionHandler
	{
		private readonly StateGraphView _graphView;

		public GraphSelectionHandler(StateGraphView graphView)
		{
			_graphView = graphView;
		}

		public void DeleteSelectedElements()
		{
			var selectedEdges = _graphView.selection.OfType<Edge>().ToList();
			var selectedStateNodes = _graphView.selection.OfType<NodeView>().ToList();

			DeleteEdges(selectedEdges);
			DeleteNodes(selectedStateNodes);
		}

		public void HandleSelectionChanged()
		{
			HandleSelectionOfScriptableObject();
			HandleSelectionOfGameObject();
		}

		private void HandleSelectionOfScriptableObject()
		{
			if (Selection.activeObject is not StateMachineModel model) return;

			ModelSelection.ActiveModel = model;
		}

		private void HandleSelectionOfGameObject()
		{
			if (!Selection.activeGameObject) return;
			if (!Selection.activeGameObject.TryGetComponent(out StateMachineController stateMachineController)) return;

			ModelSelection.ActiveModel = stateMachineController.Model;

			var stateManager = (StateNodeGraphStateManager)_graphView.StateManager;
			stateManager.SetStateControllerId(stateMachineController.Id);
		}

		private void DeleteEdges(List<Edge> edges)
		{
			var model = (StateMachineModel)_graphView.StateManager.Model;
			foreach (var edge in edges)
			{
				var transitionData = edge.userData as StateTransitionModel;
				model.RemoveTransition(transitionData);
			}

			EditorApplication.delayCall += () => { _graphView.PopulateGraph(model, false); };
		}

		private void DeleteNodes(List<NodeView> stateNodeViews)
		{
			var model = (StateMachineModel)_graphView.StateManager.Model;
			foreach (var node in stateNodeViews)
			{
				var nodeData = node.userData as StateNodeModel;
				model.RemoveState(nodeData);
			}

			EditorApplication.delayCall += () => { _graphView.PopulateGraph(model, false); };
		}
	}
}