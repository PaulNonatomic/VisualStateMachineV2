using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.Editor.Utils;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
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
			
			parent.Add(port);
			
			return port;
		}

		public static StateMachineModel UpdatePortDataInModel(StateMachineModel model)
		{
			for (var index = 0; index < model.Transitions.Count; index++)
			{
				var transition = model.Transitions[index];
				if (model.TryGetNodeById(transition.OriginNodeId, out var originNode))
				{
					transition.OriginPort = UpdateTransitionPortDataFromState(originNode, transition.OriginPort);
				}

				if (model.TryGetNodeById(transition.DestinationNodeId, out var destinationNode))
				{
					transition.DestinationPort = UpdateTransitionPortDataFromState(destinationNode, transition.DestinationPort);
				}

				model.Transitions[index] = transition;
			}

			return model;
		}
		
		public static PortModel UpdateTransitionPortDataFromState(StateNodeModel nodeModel, PortModel currentPortModel)
		{
			var stateType = nodeModel.State.GetType();
			
			var eventInfo = stateType.GetEvent(currentPortModel.Id);
			if (eventInfo == null) return currentPortModel;
			
			var attributes = eventInfo.GetCustomAttributes(typeof(TransitionAttribute), false);
			if (attributes.Length == 0) return currentPortModel;
				
			var transAtt = (TransitionAttribute) attributes[0];
			var actualPortModel = transAtt.GetPortData(eventInfo, 0);

			if (currentPortModel.PortLabel != actualPortModel.PortLabel)
			{
				currentPortModel.PortLabel = actualPortModel.PortLabel;
			}

			if (currentPortModel.FrameDelay != actualPortModel.FrameDelay)
			{
				currentPortModel.FrameDelay = actualPortModel.FrameDelay;
			}

			return currentPortModel;
		}
	}
}