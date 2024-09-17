using System.Linq;
using Nonatomic.VSM2.Logging;

namespace Nonatomic.VSM2.NodeGraph
{
	public static class NodeGraphModelUtils
	{
		public static bool TryGetPortsByIdWithIndexFallback(TransitionModel transition, NodeModel originNode, NodeModel destinationNode, out PortModel originPort, out PortModel destinationPort)
		{
			//Does node contain ports
			originPort = originNode.OutputPorts.FirstOrDefault(port => port.Id == transition.OriginPort.Id);
			destinationPort = destinationNode.InputPorts.FirstOrDefault(port => port.Id == transition.DestinationPort.Id);
					
			//If port id is missing, check if port index is correct
			originPort ??= originNode.OutputPorts.FirstOrDefault(port => port.Index == transition.OriginPort.Index);
			destinationPort ??= destinationNode.InputPorts.FirstOrDefault(port => port.Index == transition.DestinationPort.Index);
			
			//Warn if ports are missing
			if(originPort == null) GraphLog.LogWarning($"Origin port {transition.OriginPort.Id} not found on node {originNode.Id}");
			if(destinationPort == null) GraphLog.LogWarning($"Destination port {transition.DestinationPort.Id} not found on node {destinationNode.Id}");

			return (originPort != null && destinationPort != null);
		}
		
		public static void SyncTransitionPorts(TransitionModel transition, PortModel originPort, PortModel destinationPort)
		{
			//sync indices
			if (transition.OriginPort.Index != originPort.Index)
			{
				GraphLog.LogWarning($"Correcting index of TransitionData.OriginPort from {transition.OriginPort.Index} to {originPort.Index}");
				transition.OriginPort.Index = originPort.Index;
			}

			if (transition.DestinationPort.Index != destinationPort.Index)
			{
				GraphLog.LogWarning($"Correcting index of TransitionData.DestinationPort from {transition.DestinationPort.Index} to {destinationPort.Index}");
				transition.DestinationPort.Index = destinationPort.Index;
			}
						
			//sync id's
			if (transition.OriginPort.Id != originPort.Id)
			{
				GraphLog.LogWarning($"Correcting Id of TransitionData.OriginPort from {transition.OriginPort.Id} to {originPort.Id}");
				transition.OriginPort.Id = originPort.Id;
			}
						
			if (transition.DestinationPort.Id != destinationPort.Id)
			{
				GraphLog.LogWarning($"Correcting Id of TransitionData.DestinationPort from {transition.DestinationPort.Id} to {destinationPort.Id}");
				transition.DestinationPort.Id = destinationPort.Id;
			}
		}
	}
}