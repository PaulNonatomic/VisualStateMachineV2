using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.StateGraph.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateGraphContextMenu
	{
		public event Action<Vector2> OnCreateNewStateNode;
		public event Action<NodeView> OnDeleteStateNode;
		public event Action<StateNodeEdge> OnDeleteEdgeContext;
		
		private readonly NodeGraphView _graphView;

		public StateGraphContextMenu(NodeGraphView graphView)
		{
			_graphView = graphView;
			_graphView.RegisterCallback((ContextualMenuPopulateEvent evt) =>
			{
				switch (evt.target)
				{
					case StateGraphView stateGraphView:
						BuildStateGraphContext(evt);
						break;
					case StateNodeEdge stateNodeEdge:
						BuildStateEdgeContext(evt);
						break;
					case StateNodeView stateNodeView:
						BuildStateNodeContext(evt);
						break;
					case EntryNodeView entryNodeView:
						BuildEntryNodeContext(evt);
						break;
					case ExitNodeView exitNodeView:
						BuildExitStateNodeContext(evt);
						break;
				}
			});
		}

		private void BuildStateNodeContext(ContextualMenuPopulateEvent evt)
		{
			var target = evt.target;
			evt.menu.ClearItems();
			evt.menu.AppendAction("Delete", action =>
			{
				OnDeleteStateNode?.Invoke(target as StateNodeView);
			});
		}

		private void BuildExitStateNodeContext(ContextualMenuPopulateEvent evt)
		{
			var target = evt.target;
			evt.menu.ClearItems();
			evt.menu.AppendAction("Delete", action =>
			{
				OnDeleteStateNode?.Invoke(target as ExitNodeView);
			});
		}
		
		private void BuildEntryNodeContext(ContextualMenuPopulateEvent evt)
		{
			var target = evt.target;
			evt.menu.ClearItems();
		}
		
		private void BuildStateEdgeContext(ContextualMenuPopulateEvent evt)
		{
			var target = evt.target;
			
			evt.menu.ClearItems();
			evt.menu.AppendAction("Delete", action =>
			{
				OnDeleteEdgeContext?.Invoke(target as StateNodeEdge);
			});
		}

		private void BuildStateGraphContext(ContextualMenuPopulateEvent evt)
		{
			evt.menu.ClearItems();
			evt.menu.AppendAction("Add State", action =>
			{
				OnCreateNewStateNode?.Invoke(action.eventInfo.mousePosition);
			});
		}
	}
}