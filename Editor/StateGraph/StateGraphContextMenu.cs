using System;
using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.StateGraph.VisualElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public class StateGraphContextMenu
	{
		public event Action<Vector2> OnCreateNewStateNode;
		public event Action<Vector2> OnCreateNewStickyNote;
		public event Action<NodeView> OnDeleteStateNode;
		public event Action OnDeleteSelection;
		public event Action OnCopySelected;
		public event Action OnPasteSelected;
		public event Action<StateNodeEdge> OnDeleteEdgeContext;
		
		private readonly NodeGraphView _graphView;

		public StateGraphContextMenu(NodeGraphView graphView)
		{
			_graphView = graphView;
			_graphView.RegisterCallback<ContextualMenuPopulateEvent>(BuildContextMenu);
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
			evt.menu.AppendAction( "Delete", action 
				=> OnDeleteStateNode?.Invoke(nodeView));

			if (_graphView.selection.Count > 0)
			{
				evt.menu.AppendAction("Copy", action 
					=> OnCopySelected?.Invoke());
			}
			
			if (CopyPasteHelper.LastCopy != null)
			{
				evt.menu.AppendAction("Paste", action 
					=> OnPasteSelected?.Invoke());
			}
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
			
			if (_graphView.selection.Count > 0)
			{
				evt.menu.AppendAction("Copy", action 
					=> OnCopySelected?.Invoke());
				
				evt.menu.AppendAction("Delete", action 
					=> OnDeleteSelection?.Invoke());
			}

			if (CopyPasteHelper.LastCopy != null)
			{
				evt.menu.AppendAction("Paste", action 
					=> OnPasteSelected?.Invoke());
			}
		}
	}
}