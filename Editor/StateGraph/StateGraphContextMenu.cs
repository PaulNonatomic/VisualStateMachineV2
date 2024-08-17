using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateGraphContextMenu
	{
		public event Action<Vector2> OnCreateNewStateNode;
		public event Action<Vector2> OnCreateNewStickyNote;
		public event Action<Vector2> OnCreateNewStack;
		public event Action<NodeView> OnDeleteStateNode;
		public event Action<StateNodeEdge> OnDeleteEdgeContext;

		public StateGraphContextMenu(NodeGraphView graphView)
		{
			graphView.RegisterCallback<ContextualMenuPopulateEvent>(BuildContextMenu);
		}

		private void BuildContextMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.ClearItems();

			switch (evt.target)
			{
				case StateGraphView _:
					BuildStateGraphContext(evt);
					break;
				case StateNodeEdge edge:
					BuildStateEdgeContext(evt, edge);
					break;
				case NodeView nodeView:
					BuildNodeContext(evt, nodeView);
					break;
			}
		}

		private void BuildNodeContext(ContextualMenuPopulateEvent evt, NodeView nodeView)
		{
			evt.menu.AppendAction("Delete", action 
				=> OnDeleteStateNode?.Invoke(nodeView));
		}

		private void BuildStateEdgeContext(ContextualMenuPopulateEvent evt, StateNodeEdge edge)
		{
			evt.menu.AppendAction("Delete", action 
				=> OnDeleteEdgeContext?.Invoke(edge));
		}

		private void BuildStateGraphContext(ContextualMenuPopulateEvent evt)
		{
			evt.menu.AppendAction("Add State", action 
				=> OnCreateNewStateNode?.Invoke(action.eventInfo.mousePosition));
			
			evt.menu.AppendAction("Add Sticky Note", action
				=> OnCreateNewStickyNote?.Invoke(action.eventInfo.mousePosition));
			
			evt.menu.AppendAction("Add Stack", action
				=> OnCreateNewStack?.Invoke(action.eventInfo.mousePosition));
		}
	}
}