using Nonatomic.VSM2.Editor.NodeGraph;
using Nonatomic.VSM2.NodeGraph;
using Nonatomic.VSM2.StateGraph;
using Nonatomic.VSM2.StateGraph.Attributes;
using Nonatomic.VSM2.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nonatomic.VSM2.Editor.StateGraph.Factories
{
	/// <summary>
	///     Utility class for port-related operations
	/// </summary>
	public static class PortOperations
	{
		/// <summary>
		///     Creates a port on a node view
		/// </summary>
		public static Port CreatePort(
			NodeView nodeView,
			Direction direction,
			Port.Capacity capacity,
			PortModel portModel,
			IEdgeConnectorListener connectorListener)
		{
			// Create the port
			var port = nodeView.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(Node));

			// Configure the port
			ConfigurePort(port, portModel, connectorListener);

			return port;
		}

		/// <summary>
		///     Adds a port to a container element
		/// </summary>
		public static void AddPortToContainer(Port port, VisualElement container)
		{
			container.Add(port);
		}

		/// <summary>
		///     Configures a port's properties based on a port model
		/// </summary>
		private static void ConfigurePort(Port port, PortModel portModel, IEdgeConnectorListener connectorListener)
		{
			// Set basic properties
			port.name = portModel.Id;
			port.userData = portModel;

			// Add connector
			var edgeConnector = new EdgeConnector<StateNodeEdge>(connectorListener);
			port.AddManipulator(edgeConnector);

			// Set display name
			port.portName = portModel.PortLabel == default
				? StringUtils.ProcessPortName(portModel.Id)
				: StringUtils.ProcessPortName(portModel.PortLabel);

			// Set color
			if (ColorUtility.TryParseHtmlString(portModel.PortColor, out var color)) port.portColor = color;

			// Add type information to the display name if needed
			if (!string.IsNullOrEmpty(portModel.PortTypeName))
			{
				port.portName += $"<color={portModel.PortColor}><b><i><size=11><{portModel.PortTypeLabel}></color></b></i>";
				port.AddToClassList("typed");
			}
		}

		/// <summary>
		///     Applies state-specific styling to a port model
		/// </summary>
		public static void ApplyStateColorToPort(StateNodeModel nodeModel, PortModel portModel)
		{
			var stateType = nodeModel.State.GetType();

			if (AttributeUtils.TryGetInheritedCustomAttribute<NodeColorAttribute>(stateType, out var colorAtt)) portModel.PortColor = colorAtt.HexColor;
		}

		/// <summary>
		///     Checks if two ports are compatible for connection
		/// </summary>
		public static bool ArePortsCompatible(PortModel sourcePort, PortModel targetPort)
		{
			// Empty port types are compatible with anything
			var blankPortTypes = string.IsNullOrEmpty(sourcePort.PortTypeName) &&
								 string.IsNullOrEmpty(targetPort.PortTypeName);
			if (blankPortTypes) return true;

			// Otherwise types must match
			return sourcePort.PortTypeName == targetPort.PortTypeName;
		}
	}
}