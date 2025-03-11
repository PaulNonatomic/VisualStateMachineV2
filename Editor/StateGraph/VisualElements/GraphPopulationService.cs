using System.Linq;
using Nonatomic.VSM2.Editor.StateGraph.Factories;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.States;
using UnityEditor;
using UnityEngine;

namespace Nonatomic.VSM2.Editor.StateGraph.VisualElements
{
	public class GraphPopulationService
	{
		private readonly StateGraphView _graphView;

		public GraphPopulationService(StateGraphView graphView)
		{
			_graphView = graphView;
		}

		public void AddEntryNode(StateMachineModel stateModel)
		{
			if (!stateModel) return;
			if (stateModel.HasState<EntryState>()) return;
			StateGraphNodeFactory.MakeStateNodeData(stateModel, typeof(EntryState), Vector2.zero);
		}

		public void PopulateWithDelay(StateMachineModel stateModel, bool recentre)
		{
			Debug.Log($"GraphPopulationService.PopulateWithDelay");
			
			//A delay is required to allow the entry node time to be added
			EditorApplication.delayCall += () =>
			{
				//It's possible that the delayCall is invoked multiple times when entering run time
				//This just prevents adding the nodes multiple times
				if (_graphView.nodes.ToList().Count > 0) return;

				AddNodes(stateModel);
				AddEdges(stateModel);

				if (recentre) _graphView.HandleRecenter();
			};
		}

		private void AddNodes(StateMachineModel stateMachineModel)
		{
			Debug.Log($"GraphPopulationService: AddNodes");
			var stateNodes = stateMachineModel.Nodes.Cast<StateNodeModel>();
			foreach (var nodeModel in stateNodes)
			{
				var nodeView = StateGraphNodeFactory.MakeNode(_graphView, nodeModel, stateMachineModel);
				_graphView.AddElement(nodeView);
			}
		}

		private void AddEdges(StateMachineModel stateMachineModel)
		{
			foreach (var transition in stateMachineModel.Transitions) StateGraphTransitionFactory.MakeTransitionView(_graphView, transition);
		}
	}
}