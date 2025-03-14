﻿using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph
{
	public static class StateGraphPortFactory
	{
		public static Port MakePort(GraphView graphView, 
			StateMachineModel model, 
			NodeView nodeView, 
			VisualElement parent, 
			Direction direction, 
			Port.Capacity capacity, 
			PortModel portModel)
		{
			var stateNodeEdgeListener = new StateNodeEdgeListener(graphView, model);
			var edgeConnector = new EdgeConnector<StateNodeEdge>(stateNodeEdgeListener);

			var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(Node));
			port.name = portModel.Id;
			port.userData = portModel;
			port.AddManipulator(edgeConnector);
			port.portName = portModel.PortLabel == default
				? StringUtils.ProcessPortName(portModel.Id)
				: StringUtils.ProcessPortName(portModel.PortLabel);

			if (ColorUtility.TryParseHtmlString(portModel.PortColor, out var color))
			{
				port.portColor = color;
			}
			
			if (!string.IsNullOrEmpty(portModel.PortTypeName))
			{
				var colorHex = ColorUtility.ToHtmlStringRGB(port.portColor);
				port.portName += $"<color={portModel.PortColor}><b><i><size=11><{portModel.PortTypeLabel}></color></b></i>";
				port.AddToClassList("typed");
			}
			
			parent.Add(port);
			
			return port;
		}
	}
}