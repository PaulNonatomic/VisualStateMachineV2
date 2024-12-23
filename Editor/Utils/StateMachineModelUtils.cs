using System;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;

namespace Nonatomic.VSM2.Editor.Utils
{
	public static class StateMachineModelUtils
	{
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

				model.UpdateTransition(index, transition);
			}

			return model;
		}
		
		public static PortModel UpdateTransitionPortDataFromState(StateNodeModel nodeModel, PortModel currentPortModel)
		{
			if (nodeModel == null) return currentPortModel;
			if (!nodeModel.State) return currentPortModel;
			
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